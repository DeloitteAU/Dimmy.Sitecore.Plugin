$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS AppPool\DefaultAppPool", 'FullControl', @('ContainerInherit', 'ObjectInherit'), 'None', 'Allow');
$acl = Get-Acl -Path "C:\inetpub\wwwroot";
$acl.SetAccessRule($rule);
$acl | Set-Acl -Path "C:\inetpub\wwwroot";