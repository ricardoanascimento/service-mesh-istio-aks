# Summary

This project aims to demonstrate how circuit breaking and retry patterns can be offloaded from microservices and transported to a service mesh.

The project will run in an Azure Kubernetes Services Cluster with an Istio add-on and Application Gateway Ingress Controler (AGIC) configured.

## Set up Azure Kubernetes Cluster

As the intent of this project is around service mesh capabilities, I suggest creating Azure kubernetes cluster using Azure Portal. A free tier must be enough to perform all tests involved.

Once the cluster is created, follow the instructions for the existing cluster from [Deploy Istio-based service mesh add-on for Azure Kubernetes Service (preview)](https://learn.microsoft.com/en-us/azure/aks/istio-deploy-addon) and after that configure the AGIC using [Tutorial: Enable application gateway ingress controller add-on for an existing AKS cluster with an existing application gateway](https://learn.microsoft.com/en-us/azure/application-gateway/tutorial-ingress-controller-add-on-existing)

## Set up Azure Container Registry

To deploy the application, you will need an Azure Container Registry. The standard tier is enough. Once it is up and running, you have to attach the registry to the Azure Kubernetes cluster using the following command:

```
 az aks update --name myakscluster --resource-group myresourcegroup --attach-acr mycontainerregistry
```

Replace the attributes according to your environment.

## Build and push container images

I am assuming you have docker installed on the machine you are using. If not, install it, if so open a terminal and execute the command:

```
docker login crservicemeshpoc.azurecr.io
```

Note **_crservicemeshpoc_** is the name of my Azure Container Registry resource, and you have to replace it with your resource name.

Now, move your terminal to **_UserService_** path and execute the commands:

```
docker build -t crservicemeshpoc.azurecr.io/user-service:v4 .
docker push crservicemeshpoc.azurecr.io/user-service:v4
```

Also, update the registry name on the file [userService-deployment.yaml](/kubernetes/userService-deployment.yaml).

After that, move your terminal to **_VotingService_** and once again execute the commands:

```
docker build -t crservicemeshpoc.azurecr.io/voting-service:v4 .
docker push crservicemeshpoc.azurecr.io/voting-service:v4
```

Note the tag has changed, and don't forget to update the registry name on the file [userService-deployment.yaml](/kubernetes/votingService-deployment.yaml).

## Deploy Kubernetes Artifacts

If you have kubectl configure in your machine and Azure CLI, you can set the context to your Azure Kubernetes Service instance. If you don't have it installed on your device, it is easier to use Azure Cloud Shell, that have all you need.

Either way, you will have to set the context to Azure Kubernetes Service using the command:

```
az aks get-credentials --name <cluster-name> --resource-group <resource-group-name>
```

To verify you it is working, you can execute the command:

```
kubectl get pods --all-namespaces
```

Assuming you have just one AKS cluster and never connected to another one before, you can find **_aks-istio-system_** among the namespaces displayed with this command.

Now you can deploy all artifacts using the commands in the following order:

```
kubectl apply -f .\userService-deployment.yaml
kubectl apply -f .\userService-service.yaml
kubectl apply -f .\votingService-deployment.yaml
kubectl apply -f .\votingService-service.yaml
kubectl apply -f .\ingress.yaml
kubectl apply -f .\votingService-destinationRule.yaml
kubectl apply -f .\votingService-virtualService.yaml
```

## Specifics

### UserService

UserService executes an HTTP call to VotingService, the target for retry and circuit-breaking configurations.

### VotingService

To test the retry configuration, a request to **_/api/v1/user/1_** produces 200 and 504 status codes alternating between requests, so the first call returns 200, the second 504, and so on.

To be able to test the circuit-breaking configuration, every request for **_/api/v1/user/2_** produces a 500 error.

### [votingService-destinationRule.yaml](/kubernetes/votingService-destinationRule.yaml)

It is placing the role of circuit breaking. If, during 5 seconds, a service fails (500 status code) twice in a row, it opens the circuit, and subsequent requests fail straight forward during the next 1 minute.

### [votingService-virtualService.yaml](/kubernetes/votingService-virtualService.yaml)

It is placing the role of retry. Here it is essential to mention that the ingress configuration answer to **\*/api/v1/voting/\*\***, but it forwards the request to _/_ as per the annotation \*appgw.ingress.kubernetes.io/backend-path-prefix: **_/_** that is the reason with the \*match\* block configuration is:

```yaml
- match:
    - uri:
        exact: "/"
    - method:
        exact: GET
```

So when a GET to **_/_** path returns a 5xx family status code, it will retry 3, awaiting 2 seconds between attempts.
