#r @"build-packages/FAKE.5.8.4/tools/FakeLib.dll"
#r @"MyCustomTask.dll"
#load "build-utils.fsx"

open System
open System.IO
open System.Linq
open Fake.Core
open Fake.DotNet.Xamarin
open Fake.FileSystemHelper
open BuildHelpers
open MyCustomTask


//=== <Programmer> ===========================================

// Folder Paths
let solutionRootFolderTrailingSlash = "../"
let iosRootFolderTrailingSlash = "../BabyationApp/BabyationApp.iOS/"
let androidRootFolderTrailingSlash = "../BabyationApp/BabyationApp.Droid/"
let iosTestRootFolderTrailingSlash = "../"

// Solution/Project Names
let solutionName = "BabyationApp.sln"
let androidProjectName = "BabyationApp.Droid.csproj"
let iosProjectName = "BabyationApp.iOS.csproj"
let iosTestProjectName = "Test.Name"
let archiveName = ""

// Signing
let androidProdKeystorePath = "Certificate/Babyation 2.keystore"
let androidKeystorePassword = Environment.environVarOrDefault "KEYSTORE_PASS" ""
let androidKeystoreAlias = Environment.environVarOrDefault "KEYSTORE_ALIAS" ""


// Platform/Config [Rarely Changed]. Default Release.
let prodConfiguration = Environment.environVarOrDefault "BUILD_CONFIGURATION" "Release"
let debugPlatform = "iPhone"
let releasePlatform = "iPhone"

//=== </Programmer> ===========================================




//=== <CI-Engineer> ===========================================

// Environment*
let zipAlignPath = Environment.environVarOrDefault "ZIP_ALIGN_PATH" ""

let androidSdkPath = Environment.environVarOrDefault "ANDROID_SDK_PATH" ""
let androidNdkPath = Environment.environVarOrDefault "ANDROID_NDK_PATH" ""
let androidSdkTools = Environment.environVarOrDefault "ANDROID_SDK_TOOLS" ""
// Version Settings
let version = Environment.environVarOrDefault "VERSION_NUMBER" ""
let build = Environment.environVarOrDefault "BUILD_NUMBER" ""

// AppCenter Settings
let ownerName = Environment.environVarOrDefault "OWNER_NAME" ""
let androidAppName = Environment.environVarOrDefault "ANDROID_APP_NAME" ""
let iOsAppName = Environment.environVarOrDefault "IOS_APP_NAME" ""
let secretKey = Environment.environVarOrDefault "SECRET_KEY" ""
let distributionGroup = Environment.environVarOrDefault "DISTRIBUTION_GROUP" ""


//=== </CI-Engineer> ===========================================

//=== AppCenter command-line

//==  ===========================================
//    End of Configuration
//==  ===========================================


// TODO: Move HockeyApp deployment into scripts 
// TODO: IOS UI Test integration [need to update ui test booting to properly change platforms]
// TODO: Android UI Test [currently only does unit testing]


// Prepare Global Variables
let versionNumber = (version + "." + build)
let solutionPath = solutionRootFolderTrailingSlash + solutionName
let androidProjectPath = androidRootFolderTrailingSlash + androidProjectName

let UploadToAppCenter secretKey ownerName appName filePath distributionGroup =
    let curlApp = "curl"
    let appCenterStartUpload = "-X POST --header 'Content-Type: application/json' --header 'Accept: application/json' --header 'X-API-Token: {0}' 'https://api.appcenter.ms/v0.1/apps/{1}/{2}/release_uploads'"
    let hockeyAppUpload = "-F \"ipa=@{0}\" {1}"
    let appCenterSubmitRelease = "-X PATCH --header 'Content-Type: application/json' --header 'Accept: application/json' --header 'X-API-Token: {0}' -d '{{ \"status\": \"committed\" }}' 'https://api.appcenter.ms/v0.1/apps/{1}/{2}/release_uploads/{3}'"
    let appCenterDistributeRelease = "-X PATCH --header 'Content-Type: application/json' --header 'Accept: application/json' --header 'X-API-Token: {0}' -d '{{ \"destination_name\": \"{3}\", \"release_notes\": \"Example new release via the APIs\" }}' 'https://api.appcenter.ms/v0.1/apps/{1}/{2}/releases/{4}'"
    let curlCommand = String.Format(appCenterStartUpload, secretKey, ownerName, appName)
    Trace.trace curlCommand
    let output = HttpCallUrl.MakeCall(curlApp, curlCommand)
    Trace.trace output
    let uploadId = HttpCallUrl.GetBuildId(output)
    let uploadUrl = HttpCallUrl.GetUploadUrl(output)
    let uploadCommand = String.Format(hockeyAppUpload, filePath, uploadUrl)
    let output = HttpCallUrl.MakeCall(curlApp, uploadCommand)
    Trace.trace output
    let patchCommand = String.Format(appCenterSubmitRelease, secretKey, ownerName, appName, uploadId)
    let output = HttpCallUrl.MakeCall(curlApp, patchCommand)
    let releaseId = HttpCallUrl.GetReleaseId(output)
    Trace.trace output
    let distributeCommand = String.Format(appCenterDistributeRelease, secretKey, ownerName, appName, distributionGroup, releaseId)
    let output = HttpCallUrl.MakeCall(curlApp, distributeCommand)
    Trace.trace output

// ios-release
// Builds the project in release mode
Target.create "ios-build-appcenter" (fun _ ->
    BuildHelpers.RestorePackages solutionPath


    BuildHelpers.UpdatePlist version versionNumber prodConfiguration iosRootFolderTrailingSlash
    let iosProj = Path.Combine(iosRootFolderTrailingSlash, iosProjectName)

    iOSBuild(fun defaults ->
        {defaults with
            BuildIpa = true
            ProjectPath = iosProj
            Configuration = prodConfiguration
            Platform = releasePlatform
            Target = "Build"
        })

    let outputFolder = Path.Combine(iosRootFolderTrailingSlash, "bin", releasePlatform, prodConfiguration)
    let appPath = Directory.EnumerateFiles(outputFolder, "*.ipa", SearchOption.AllDirectories).First()
    let ipaFullPath = Path.Combine(currentDirectory, appPath)
    Trace.trace ipaFullPath

    UploadToAppCenter secretKey ownerName iOsAppName ipaFullPath distributionGroup
//TeamCityHelper.PublishArtifact ipaFullPath
)

//Target "ios-build-prod-fabric" (fun () ->
//    RestorePackages solutionPath
//
////    UpdatePlist version versionNumber prodConfiguration iosRootFolderTrailingSlash
//
//    iOSBuild (fun defaults ->
//        {defaults with
//            BuildIpa = true
//            ProjectPath = solutionRootFolderTrailingSlash + solutionName
//            Configuration = prodConfiguration
//            Platform = releasePlatform
//            Target = "Build"
//            //Properties = ["DefineConstants","__UNIFIED__,__MOBILE__,__IOS__"] // if shared project, constants may not cascade
//        })
//    
//    let outputFolder = Path.Combine(iosRootFolderTrailingSlash, "bin", releasePlatform, prodConfiguration)
//    let appPath = Directory.EnumerateFiles(outputFolder, "*.ipa", SearchOption.AllDirectories).First()
//    let ipaFullPath = Path.Combine(currentDirectory, appPath)
//
//    TeamCityHelper.PublishArtifact ipaFullPath
//
//    let objFolder = Path.Combine(iosRootFolderTrailingSlash, "obj", releasePlatform, prodConfiguration)
//    let crashlytics = Path.Combine(objFolder, "mtouch-cache", "Crashlytics.framework", "submit")
//    let betaArgs = String.Format("{0} {1} -ipaPath \"{2}\" -notifications YES -groupAliases ezcall-xamarin", fabricApiKey, fabricBuildSecret, appPath)
//
//    Exec crashlytics betaArgs
//
//    let dSYMPath = Directory.EnumerateDirectories(outputFolder, "*.dSYM").First()
//    let uploadSymbols = Path.Combine("/Applications/Fabric.app/Contents/MacOS/", "upload-symbols")
//    let symArgs = String.Format("-a {0} -p ios {1}", fabricApiKey, dSYMPath)
//    Exec uploadSymbols symArgs
//)

//Target "ios-fabric-prod" (fun () ->
//
//    let outputFolder = Path.Combine(iosRootFolderTrailingSlash, "bin", releasePlatform, prodConfiguration)
//    let objFolder = Path.Combine(iosRootFolderTrailingSlash, "obj", releasePlatform, prodConfiguration)
//    let appPath = Directory.EnumerateFiles(outputFolder, "*.ipa", SearchOption.AllDirectories).First()
//    let crashlytics = Path.Combine(objFolder, "mtouch-cache", "Crashlytics.framework", "submit")
//    let betaArgs = String.Format("{0} {1} -ipaPath \"{2}\" -notifications YES -groupAliases ezcall-xamarin", fabricApiKey, fabricBuildSecret, appPath)
//    
//    Exec crashlytics betaArgs
//
//    let dSYMPath = Directory.EnumerateDirectories(outputFolder, "*.dSYM").First()
//    let uploadSymbols = Path.Combine("/Applications/Fabric.app/Contents/MacOS/", "upload-symbols")
//    let symArgs = String.Format("-a {0} -p ios {1}", fabricApiKey, dSYMPath)
//    Exec uploadSymbols symArgs
//)


Target.create "android-build-appcenter" (fun _ ->
    RestorePackages solutionPath

    UpdateManifest version build prodConfiguration androidRootFolderTrailingSlash

    let outPath =  Path.Combine(androidRootFolderTrailingSlash, "bin", prodConfiguration)


    let outFile = AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = androidProjectPath
            Configuration = prodConfiguration
            OutputPath = outPath
            Properties = [ ("AndroidSdkDirectory", androidSdkPath); ("AndroidNdkDirectory", androidNdkPath) ]
        }) 

    let signedFileFunc = AndroidSignAndAlign (fun defaults ->
        {defaults with
            KeystorePath = androidProdKeystorePath
            KeystorePassword = androidKeystorePassword
            KeystoreAlias = androidKeystoreAlias
            ZipalignPath = zipAlignPath
       })

    let signedFile = signedFileFunc outFile

    //TeamCityHelper.PublishArtifact signedFile.FullName

    UploadToAppCenter secretKey ownerName androidAppName signedFile.FullName distributionGroup
)


Target.runOrDefault "ios-build-appcenter"
