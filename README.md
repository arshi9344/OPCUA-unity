# OPC UA Unity Project - Quick Start Guide

## 1. Requirements

### Python Server:
- Python 3.8 or higher
- `asyncua` library (`pip install asyncua`)

### Unity Project:
- Unity 2020.3 or later
- Windows 10 or 11
- .NET Framework 4.x

### Optional:
- [UAExpert](https://www.unified-automation.com/products/development-tools/uaexpert.html) to monitor OPC UA connections

---

## 2. Set Up Python Server
- Install Python 3.8 or higher if you don’t have it already.
- Install the required library by running the following command:
  ```bash
  pip install asyncua
  
---
Run the provided Python server code:
```bash
python opc_server.py
```

----
3. Create a Unity Project
- **Template**: 3D (Built-in Pipeline)
- **Assets Folder**: Copy the provided Assets folder into your Unity project.
>**Note**: The included DLLs are built for Windows 11.
----
4. Running and Testing
- Play: Hit Play in Unity to connect to the OPC UA server and read real-time robot joint data.
----
5. Monitoring with UAExpert (Optional)
- Download and install UAExpert.
- Launch UAExpert and add the OPC UA server using the endpoint:
opc.tcp://localhost:4840/server/
- You can browse and monitor real-time variable changes in UAExpert.
---
**Quick Troubleshooting**
- **Connection Issues?** Ensure the Python server is running with the correct endpoint: opc.tcp://localhost:4840/server/.

- **NodeId Errors?** Check that the NodeIds match between the server and Unity client.
