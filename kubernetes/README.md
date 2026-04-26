# Kubernetes Deployment Guide

## Prerequisites
- Kubernetes cluster (v1.24+)
- kubectl configured
- Persistent volume provisioner

## Deployment Steps

### 1. Create Namespace (Optional)
```bash
kubectl create namespace federation-platform
```

### 2. Apply Configuration
```bash
# Deploy all resources
kubectl apply -f deployment.yaml

# Or deploy to specific namespace
kubectl apply -f deployment.yaml -n federation-platform
```

### 3. Verify Deployment
```bash
# Check pods
kubectl get pods

# Check services
kubectl get services

# Check deployment status
kubectl rollout status deployment/federation-platform
```

### 4. Access Application
```bash
# Get external IP
kubectl get service federation-platform-service

# Access via LoadBalancer IP
curl http://<EXTERNAL-IP>/health
```

## Scaling

### Manual Scaling
```bash
kubectl scale deployment federation-platform --replicas=5
```

### Auto-scaling
HPA is configured to scale between 2-10 replicas based on CPU/Memory usage.

## Monitoring
```bash
# View logs
kubectl logs -f deployment/federation-platform

# View events
kubectl get events --sort-by=.metadata.creationTimestamp

# Check resource usage
kubectl top pods
```

## Updating
```bash
# Update image
kubectl set image deployment/federation-platform web=ghcr.io/your-org/federation-platform:v2.0

# Rollback
kubectl rollout undo deployment/federation-platform
```
