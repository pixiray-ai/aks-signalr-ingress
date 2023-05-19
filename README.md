# Sample SignalR Chat for AKS and NGINX Ingress Debugging

Demo Repository Explaining Basic Concepts on Signal-R Ingress for AKS

About the Project:
---
- ASP.NET CORE 7.0.300-preview
- SIGNALR 6.0.1
- Razorpages
- Ubuntu WSL2 22.04.2

Features:
---
- Serilog Logger for SignalR in Console of the POD
- SignalR Chat for Debugging Purposes

- Webclient Index.cshtml & chat.js
  - Running Port 80
- Signal ChatHub 
  - Running Port 5000
- Read the Logs from Docker Container
  - 'docker logs --since=1h CONTAINERID'



