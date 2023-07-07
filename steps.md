## Dry Run Deploy
Validate the configuration file is correct.
```bash
kubectl apply --dry-run=server -n default -f k8s/app.yaml
```

## Deploy
Apply the configuration file.
```bash
kubectl apply -n default -f k8s/app.yaml
```

## Check objects
query their existence
```bash
kubectl get svc weatherapi-service
kubectl get pdb weatherapi-pdb
kubectl get deployment weatherapi-deployment
kubectl get pods -l app=weatherapi-app
```

watch pods & deployments
```bash
kubectl get deployment weatherapi-deployment --watch
```
```bash
kubectl get pods -l app=weatherapi-app --watch
```

## Port Forward
Access the application locally
```bash
kubectl port-forward svc/weatherapi-service 8080:443
```

## Access Swagger Docs
Open this url in the browser: https://localhost:8080/swagger

## Optional: Watch Deployment Events
Watch deployment events 
```bash
kubectl events --for deployment/weatherapi-deployment --watch
```




## Clean Up
Delete all k8s objects.
```bash
kubectl delete deployment weatherapi-deployment
kubectl delete svc weatherapi-service
kubectl delete pdb weatherapi-pdb
```