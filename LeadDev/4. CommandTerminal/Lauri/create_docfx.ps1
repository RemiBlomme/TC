docfx init -y
$json = Get-Content docfx.json | ConvertFrom-Json
$json.metadata[0].src[0].src = "./"
$json.build.globalMetadata._appName = Split-Path -Path (Get-Location) -Leaf
$json.build.globalMetadata._appTitle = Split-Path -Path (Get-Location) -Leaf
$json | ConvertTo-Json -depth 32 | Out-File docfx.json
docfx docfx.json --serve
