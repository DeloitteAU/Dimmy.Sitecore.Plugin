![DimSim](/Dimmy.Sitecore.png) 

# Dimmy.Sitecore.Plugin

Dimmy is a tool to help you with a docker based development workflow, and the Sitecore plugin helps you do this when building Sitecore Applications.

## Getting started

To get started there are a few prerequisites:
* [Dimmy](https://github.com/gravypower/Dimmy) installed
* Docker
* Docker Compose
* A Sitecore Licence file
* Access to a docker registry with the [Sitecore Docker images](https://github.com/Sitecore/docker-images)
* Some soy sauce

With that out of the way we can install the plugin and get started

`dimmy plugins install --package-id Dimmy.Sitecore.Plugin --package-version 1.0.1`
See the [nuget page](https://www.nuget.org/packages/Dimmy.Sitecore.Plugin/) for the latest version, or run `dimmy plugins ls --remote`

This plugin extends the Dimmy project initialise command with some Sitecore flavoured docker-compose templates and development settings.

`dimmy project init sitecore-10.0.0 --name SomeClient --source-code-path C:\projects\SomeClient --working-path C:\clients\SomeClient --license-path C:\license.xml --topology xm1`

A few things to ensure:

* If you are targeting 9.3.0 and below you need to pass in a registry `--registry my-sitecore-containers.azurecr.io` for example. Sitecore have only relesed 10.0.0 images.
* can pull from the container registry.
* `source-code-path` and `working-path` exist.
  
Now all you need to do is navigate to the `working-path` and run `dimmy project start`

## What is the plugin doing?

On Init:
* Copies a crafted docker-compose templet to the source-code-path, based on the selected topology.
* The supplied licence file is encoded and add into the non-public settings.
* Generates a SQL SA password and add into the non-public settings.
* Generates a TelerikEncryptionKey and add into the non-public settings.
* Adds entries for each of the ports for each of the Sitecore roles into the non-public settings,
* Adds in docker images for each of the roles into the public settings

On Start:
* generates new certificates for the public facing roles from the dimmy Certificate Authority
* generates traefik configuration

Currently, this plugin supports:
* Sitecore 9.3
* Sitecore 10.0.0

The  aim is to support all versions the Sitecore Docker Images build scripts build.

## Whats Next

No official road map yet but you can see what [enhancements](https://github.com/DeloitteDigitalAPAC/Dimmy.Sitecore.Plugin/labels/enhancement) may be on the way. If you wish to contribute, please, by all means, open a PR. Also goes for any bugs you may find open a new git issue.


## Who is Deloitte Digital?

**Part Business. Part Creative. Part Technology. One hundred per cent digital.**

Pioneered in Australia, Deloitte Digital is committed to helping clients unlock the business value of emerging technologies. We provide clients with a full suite of digital services, covering digital strategy, user experience, content, creative, engineering and implementation across mobile, web and social media channels.

[http://www.deloittedigital.com/au](http://www.deloittedigital.com/au)

## LICENSE (BSD-3-Clause)
[View License](LICENSE)

## 終わり

This is not much more than a thought experiment currently, use at your own risk and thank you for taking the time to look at this project. 

Made with :heart: in Bendigo and Melbourne.
