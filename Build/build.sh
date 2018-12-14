#!/bin/bash

mono build-tools/NuGet/nuget.exe install NUnit.Runners -Version 2.6.4
mono build-tools/NuGet/nuget.exe install FAKE -Version 5.8.4
mono build-packages/FAKE.5.8.4/tools/FAKE.exe build.fsx $@