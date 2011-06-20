param($installPath, $toolsPath, $package, $project)
$project.Object.References | Where-Object { $_.Name -eq 'Machine.Specifications.TDNetRunner' } | ForEach-Object { $_.Remove() }

if ($false -eq $false)
{
  Write-Warning "This is the unsigned version of Machine.Specifications. Use 'Install-Package Machine.Specifications-Signed' to install the signed version."
}