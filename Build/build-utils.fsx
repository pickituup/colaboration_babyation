module BuildHelpers

open Fake
open Fake.XamarinHelper
open Fake.FileHelper
open System
open System.IO
open System.Linq

let Exec command args =
    let result = Shell.Exec(command, args)

    if result <> 0 then failwithf "%s exited with error %d" command result

let RestorePackages solutionFile =
    Exec "build-tools/NuGet/NuGet.exe" ("restore " + solutionFile)
    //solutionFile |> RestoreComponents (fun defaults -> {defaults with ToolPath = "build-tools/xpkg/xamarin-component.exe" })

let UpdatePlist shortVersion version configuration project =
    let info = Path.Combine(project, "Info.plist")
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleShortVersionString " + shortVersion + "' " + info)
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleVersion " + version + "' " + info)

let UpdateManifest version build configuration project =
    let path = (project + "/Properties/AndroidManifest.xml")
    let ns = Seq.singleton(("android", "http://schemas.android.com/apk/res/android"))
    XmlPokeNS path ns "manifest/@android:versionName" (version + "." + build)
    XmlPokeNS path ns "manifest/@android:versionCode" build
    //let path = (project + "/Assets/crashlytics-build_" + configuration + ".properties")
    //ReplaceInFiles [("version_name=1.0", "version_name=" + (version + "." + build)); ("version_code=1", "version_code=" + build)] [path]

let RunNUnitTests dllPath xmlPath =
    Exec "/Library/Frameworks/Mono.framework/Versions/Current/bin/nunit-console4" (dllPath + " -xml=" + xmlPath)
