set pkgversion=%1
msbuild /T:Build /P:Configuration=Release MefOutOfBand.sln
mkdir distrib
.nuget\NuGet.exe pack -Version %pkgversion% src\System.ComponentModel.Composition.Lightweight\Microsoft.Mef.Lightweight.nuspec -OutputDirectory distrib
.nuget\NuGet.exe pack -Version %pkgversion% src\System.ComponentModel.Composition.Web.Mvc\Microsoft.Mef.MvcCompositionProvider.nuspec -OutputDirectory distrib
