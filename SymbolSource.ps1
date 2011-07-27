$version = "1.0.64355.1"

function Search-And-Replace ($path, $lookup) {
    Write-Output ("Replacing in {0}" -f $path)
    Get-ChildItem -Path $path -Recurse -Exclude .git -Include *.cs | ForEach-Object { 
        Write-Verbose "`t$_";
        (Get-Content $_) | ForEach-Object {
            Write-Debug "`t`t`t$_"
            $line = $_
            $lookup.GetEnumerator() | ForEach-Object {
                Write-Debug ("`t`t`t`tReplacing {0} with {1} in {2}" -f $_.Key, $_.Value, $line)
                $line = $line -replace $_.Key, $_.Value
            }
            $line
        } | Set-Content $_ 
    }
    Write-Output "Done"
}


Search-And-Replace . @{
    '  (unsafe )*((private|internal|protected) )*(internal )*(((sealed|partial|abstract|partial) )*)(class|struct|interface|enum) ' = '  public $1$5$8 '
}

echo "msbuild Build\ccimetadata.build /p:CCNetLabel=$version"
echo ".\NuGet pack SymbolSource.nuspec -version $version -symbols"

exit

(
    "  protected internal sealed partial class ",
    "  internal abstract partial class ",
    "  class ",
    "  unsafe internal struct "
) | ForEach-Object {
    $_ -replace '  (unsafe )*((private|internal|protected) )*(internal )*(((sealed|partial|abstract|partial) )*)(class|struct|interface|enum) ', '  public $1$5$8 ' }

exit;