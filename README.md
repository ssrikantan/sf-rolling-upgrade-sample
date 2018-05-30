# Sample Application that showcases Azure Service Fabric Application Rolling upgrade

# This sample demonstrates the following features of Azure Service Fabric
	- Support for deploying containerized Applications with Service Fabric as the orchestrator.
	- Surface Docker Health status in the overall health report of the Service Fabric Cluster
	- Service discovery using the inbuilt Service Fabric DNS
	- Perform Application Rolling upgrades

# The sample comprises of 2 Applications:
	- A Web Application (sfaspnetsample) calls a REST API (sfwebapi) - both are built using ASP.NET Core 2.0 and packaged as Docker Containers for Linux, using Visual Studio 2017. 
	- The REST API has a method that returns the hostname on which it is running. The Web Application does nothing but print the host name returned by the REST API and the host name that itself runs on
	- The Web Application reads the Service DNS name of the REST API from its appsettings.json file, and uses Service Fabric DNS Service to resolve it at run time.
	- The Web Application implements Docker Health Check and uses a retry policy with timeouts
	- The Web Application runs on Host port 80 and the REST API on Host port 5002
	
# Software Pre requisites to run this sample:
1. Git Bash running on a Windows 10 Laptop was used to test the sample in this article.
2. Service Fabric SDK and tools for Visual Studio 2017 were installed, as described here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started
3. Yeoman generator for Windows was installed to generate the Application and Service manifest files and Package the Solution used in this sample. Details here -  https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started-linux#set-up-yeoman-generators-for-containers-and-guest-executables
4. Azure Service Fabric CLI, as described here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cli
5. Docker for Windows on the local dev Machine

# Creating the Service Fabric Cluster:
Use the Azure portal to create a Single Node Type Cluster running Ubuntu. 
* After the Cluster is created, create a Self-signed certificate (for demo use) on the Current User Store on the local machine. Register the thumbprint of this certificate in the Service Fabric Cluster. These steps are described in detail here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cluster-creation-via-portal. 
* Connect to the cluster by following the steps described here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-connect-to-secure-cluster. 
* The Client certificate needs to be in the .pem format. Refer to https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-cli to convert a .pfx file to a .pem format


