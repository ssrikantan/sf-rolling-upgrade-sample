# Sample Application that showcases Azure Service Fabric Application Rolling upgrade

This sample demonstrates the following features of Azure Service Fabric
* Support for deploying containerized Applications with Service Fabric as the orchestrator.
* Surface Docker Health status in the overall health report of the Service Fabric Cluster
* Service discovery using the inbuilt Service Fabric DNS
* Perform Application Rolling upgrades

The sample comprises of 2 Applications: (included in this repo)
* A Web Application (sfaspnetsample) calls a REST API (sfwebapi) - both are built using ASP.NET Core 2.0 and packaged as Docker Containers for Linux, using Visual Studio 2017. 
* The REST API has a method that returns the hostname on which it is running. The Web Application does nothing but print the host name returned by the REST API and the host name that itself runs on
* The Web Application reads the Service DNS name of the REST API from its appsettings.json file, and uses Service Fabric DNS Service to resolve it at run time.
* The Web Application runs on Host port 80 and the REST API on Host port 5002
	
The screenshot below shows the Docker compose file with the Halthcheck configuration used, and the entries in the appsettings.json file of the Web Application where the Service name for the REST API is read, for Service discovery.
![GitHub Logo](/images/dockerhealth.png)

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

# Creating the Container images for the Web and API Services and uploading them to Azure Container Registry:
The Visual Studio Solution for each of the Applications have a Docker Compose Package. Build the Solution locally to create the Docker Container images. The Container images that would be created are sfwebapp and sfwebapi. Tag and push them to Azure Container Registry, as Version01 of the Sample Solution. At the end of this step, there would be 2 Container images uploaded to Azure Container Registry(ACR):
sfwebacr.azurecr.io/sfwebapp:web1rev01   (for the Web App)
sfwebacr.azurecr.io/sfwebapi:web2rev01     (for the API App)

Three versions of the container images are used in this sample, and uploaded to ACR. The changes that were made in the Source files and the expected outcomes from those changes are:

| Version of App | Version -Container image | Code changes in each version of the app |
|----------------|----------------------------|-----------------------------------------|
|rev01           |sfwebacr.azurecr.io/ sfwebapp:web1rev01  |First version of the Application that would be deployed to Azure Service Fabric |
|rev01           |sfwebacr.azurecr.io/ sfwebapi:web2rev01  | First version of the Application that would be deployed to Azure Service Fabric |
| rev02          | sfwebacr.azurecr.io/ sfwebapp:web1rev02 | 1) A delay is induced in the Startup of the Web Application (Program.cs file). The Thread sleeps for 90 seconds. This is to mimic a scenario where the Application needs to warm up first before it can cater to requests. The Docker Heath check would fail initially, the ASF Application Rolling upgrade would wait for a pre-determined duration. Once the Web Application starts, the Docker health check returns a 'healthy' status. The Rolling upgrade process then continues to Rollforward to the next fault domain. In all this time, a Web Application would continue to be available, ensuring zero downtime during the Rolling upgrade process. 2) In the index.html, the version number string is set to Version02 - to verify that the right version is deployed after the rolling upgrade |
| rev02          | sfwebacr.azurecr.io/ sfwebapi:web2rev02 | The ValuesController.cs class - Version number string returned in the response changed to Version02 |
| rev03          |sfwebacr.azurecr.io/ sfwebapp:web1rev03  | 1) A delay is induced in the Startup of the Web Application (Program.cs file). The Thread sleeps for 10 minutes. This is to mimic a scenario where the Application needs to warm up first before it can cater to requests. The Docker Heath check would fail initially, the ASF Application Rolling upgrade would wait for a pre-determined duration. Since the timeout specified during the Rolling upgrade elapses before 10 minutes (the Docker Health check continues to returns 'unhealthy' during this time) The Rolling upgrade starts a Rollback to the previous version (which is Version02) of the Application. In all this time, a Web Application would continue to be available, ensuring zero downtime during the Rolling upgrade process. 2) In the index.html, the version number string is set to Version03 - to test that the right version is deployed after the rolling upgrade |
| rev03          | sfwebacr.azurecr.io/ sfwebapi:web2rev03  | The ValuesController.cs class - Version number string returned in the response changed to Version03 |

# Package the applications using Yeoman Generator:
The steps performed are as documented here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started-containers-linux. Create a single Application that packages each of the Web and API Apps as Service Types. When prompted, provide the url to the container images that were uploaded to ACR in the previous steps. 

In order to easily run this sample application for all 3 scenarios, after the Yeoman tool created the app packages with the defaults, 3 separate folders with app packages were created that contain configurations added to each version of the Application. These root folders are named sf2tierdemoappVersion1, sf2tierdemoappVersion2, and sf2tierdemoappVersion3.
When Version1 of the Package has to be deployed, rename folder *sf2tierdemoappVersion1* to *sf2tierdemoapp* and then run install.sh in that folder.

# Deploy Version01 of the Application - new deployment to SF Cluster
Before deploying the Application package, ensure that the Service Fabric Cluster is up and running by launching the SF Explorer. Make changes to the Application and Service Manifest files as necessary (e.g. the sample deploys to a NodeType names 'sfclmain' as a part of placement constraints. This would need to be changed to suit your deployment, etc. etc.)

1. Connect to the Service fabric cluster from CLI
sfctl cluster select --endpoint https://dockersfcluster.southindia.cloudapp.azure.com:19080 --pem dockersfclustercert.pem --no-verify

2. Run the install command
install.sh   
3. Check the Service fabric cluster to view the application deployed. See screenshot below
![GitHub Logo](/images/webapp_deployed.png)

# Perform Application Rolling Upgrade to Version02
1. Use the right package folder to run the Rolling upgrade from.

rename folder *sf2tierdemoappVersion2* to *sf2tierdemoapp* 

2. Upload the package and provision the Application type in SF

sfctl application upload --path ~/source/repos/sf2tierclusterapp/sf2tierdemoapp/sf2tierdemoapp

sfctl application provision --application-type-build-path sf2tierdemoapp --debug

(optional, for cleanup - sfctl store delete --content-path sf2tierdemoapp)

3. Call the Rolling upgrade using the sfctl commands

sfctl application upgrade --application-name fabric:/sf2tierdemoapp  --application-version Version2.0 --parameters "{}"  --mode Monitored  --health-check-wait-duration  PT0H03M0S --health-check-retry-timeout PT0H01M0S --warning-as-error --failure-action rollback

--warning-as-error -> ensures that when Docker health check returns 'unhealthy', setting this parameter results in the Health of the Service Fabric Application as 'Unhealthy' and shows up as 'error'.


--health-check-wait-duration  -> The upgrade process waits for the duration specified here after the package is deployed. 

Initially, the SF Application Health shows up as 'unhealthy' when the Docker Health check returns 'unhealthy'. When the Application Start up thread resumes, Docker returns a 'healthy' status, thereby the SF Application cluster status changes to 'healthy'. Once the wait duration elapses, the Upgrade process executes a Roll forward to the next Upgrade Domain. This continues till the Roll forward is completed for all the remaining Upgrade domains.

Hitting the application URL during the upgrade process would return Version01 of the Web page initially, and as and when the first Upgrade Domain has been upgraded, then a combination of Version01 and Version02 of the Web Application pages would be returned. When all of the Upgrade domains are done, then only Version02 of the Web pages would be returned.

The screenshot below shows how the Docker Health status shows up on the Service Fabric Explorer
![GitHub Logo](/images/sfexplorerstate1.png)

# Perform Application Rolling Upgrade to Version03
1. Use the right package folder to run the Rolling upgrade from.

rename folder *sf2tierdemoappVersion3* to *sf2tierdemoapp* 

2. Upload the package and provision the Application type in SF

sfctl application upload --path ~/source/repos/sf2tierclusterapp/sf2tierdemoapp/sf2tierdemoapp

sfctl application provision --application-type-build-path sf2tierdemoapp --debug

(optional, for cleanup - sfctl store delete --content-path sf2tierdemoapp)

3. Call the Rolling upgrade using the sfctl commands

sfctl application upgrade --application-name fabric:/sf2tierdemoapp  --application-version Version3.0 --parameters "{}"  --mode Monitored  --health-check-wait-duration  PT0H02M0S --health-check-retry-timeout PT0H01M0S --warning-as-error --failure-action rollback

The SF Application Health continues to show up as 'unhealthy', since the Start up thread in the Web application is set to sleep for an inordinate duration. Once the wait duration elapses, the Upgrade process detects that the upgrade is a failure, and triggers a rollback to the previous versionl which is Version02.

Hitting the application URL during the upgrade process would return only Version02 of the Web page till the Rollback action completes.
The screenshot below shows how Rolling upgrade triggers a Rollback after the Service Health check fails after an unsuccessful upgrade to Version03 in the first upgrade domain
![GitHub Logo](/images/sfexplorerstate2.png)

# Looking at the Manifest files used in this sample application
These screenshots are annotated to illustrate certain configurations that have been implemented in this sample solution.
![GitHub Logo](/images/appmanifest.png)

The screenshot below shows how placement constraints can be specified in the Service Manifest to indicate which Node Type this Service needs to be deployed to in the SF Cluster.
![GitHub Logo](/images/servicemanifest.png)
