properties {
    $projectName = "Parsley"
    
    $projectConfig = "Release"
    $base_dir = resolve-path .\
    $source_dir = "$base_dir\src"
	
    $build_dir = "$base_dir\build"
    $unittest_dir = "$base_dir\src\Parsley.Test\bin\$projectConfig"
    $package_dir = "$build_dir\package"	

    $defaultVersion = "1.0.0.0"
}

$framework = '4.0'

FormatTaskName "------------------------------
-- {0}
------------------------------"

task default -depends Compile, Test, Package

task Init {
    delete_directory $build_dir
    create_directory $build_dir
}

task CommonAssemblyInfo {
    if(-not $version)
    {
        $version = $defaultVersion
    }
    create-commonAssemblyInfo "$version" $projectName "$source_dir\CommonAssemblyInfo.cs"
}

task Compile -depends Init, CommonAssemblyInfo {
    msbuild /t:clean /v:q /nologo /p:Configuration=$projectConfig $source_dir\$projectName.sln
    msbuild /t:build /v:q /nologo /p:Configuration=$projectConfig $source_dir\$projectName.sln
}

task Test -depends Compile {
    exec {
        & $base_dir\tools\xunit.runners.1.9.0.1566\xunit.console.clr4.exe "$unittest_dir\$projectName.Test.dll"
    }
}

task Package -depends Compile {
    delete_directory $package_dir

    copy_files "$source_dir\Parsley\bin\$projectConfig\" $package_dir
    Copy-Item "$base_dir\README.md" "$package_dir\README.txt"

    write-host "Created deployment package: $package_dir" -ForegroundColor Green
}

function global:copy_files($source,$destination,$exclude=@()){    
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude | Copy-Item -Destination {Join-Path $destination $_.FullName.Substring($source.length)} 
}

function global:delete_file($file) {
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}

function global:delete_directory($directory_name)
{
    rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:create_directory($directory_name)
{
    mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function global:create-commonAssemblyInfo($version,$applicationName,$filename)
{
    $date = Get-Date
"using System;
using System.Reflection;
using System.Runtime.InteropServices;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: ComVisible(false)]
[assembly: AssemblyVersion(""$version"")]
[assembly: AssemblyFileVersion(""$version"")]
[assembly: AssemblyCopyright(""Copyright Patrick Lioi 2011-" + $date.Year + """)]
[assembly: AssemblyProduct(""$applicationName"")]
[assembly: AssemblyConfiguration(""release"")]
[assembly: AssemblyInformationalVersion(""$version"")]"  | out-file $filename -encoding "ASCII"    
}