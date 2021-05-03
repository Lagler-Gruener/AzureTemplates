<#
    .SYNOPSIS
        
    .DESCRIPTION
        
    .EXAMPLE
        
    .NOTES  
        
#>

[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [ValidateSet("Get","Add","Remove")]
    [string]
    $Action,
    [Parameter()]
    [string]
    $sAMAccountName = "",
    [Parameter()]
    [string]
    $givenName = "",
    [Parameter()]
    [string]
    $sn = "",
    [Parameter()]
    [string]
    $userPrincipalName = ""
)

#######################################################################################################################
#region define global variables

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

#endregion
#######################################################################################################################

#######################################################################################################################
#region Script start

    #region section select subscription
    try 
    {            
        switch ($Action) {
            Add {
                Write-Output "Add new User"

                try {
                    New-ADUser -SamAccountName $sAMAccountName -GivenName $givenName -Name $sn -UserPrincipalName $userPrincipalName `
                               -Enabled $true -AccountPassword (ConvertTo-SecureString "P@ssw0rd" -AsPlainText -force) -passThru   

                    Write-Output "success"
                }
                catch {
                    Write-Output "Error: $($_.Exception.Message)"
                }                
            }
            Get {
                if($sAMAccountName -eq "")
                {
                    Write-Output "Get all User"

                    try {                     
                        $users = Get-ADUser -Filter *
                        foreach ($user in $users)
                            {
                                Write-Output "sAMAccountName: $($user.sAMAccountName)"
                                Write-Output "givenName: $($user.givenName)"
                                Write-Output "sn: $($user.sn)"
                                Write-Output "distinguishedName: $($user.distinguishedName)"
                                Write-Output "userPrincipalName: $($user.userPrincipalName)"
                                Write-Output "--------------------------------------"
                            }
                    }
                    catch {
                        Write-Output "Error: $($_.Exception.Message)"
                    } 
                }
                else {
                    Write-Output "Get User with sAMAccountName $sAMAccountName"

                    try {
                        $user = Get-ADUser -Filter "sAMAccountName -eq '$sAMAccountName'"
                        Write-Output "sAMAccountName: $($user.sAMAccountName)"
                        Write-Output "givenName: $($user.givenName)"
                        Write-Output "sn: $($user.sn)"
                        Write-Output "distinguishedName: $($user.distinguishedName)"
                        Write-Output "userPrincipalName: $($user.userPrincipalName)"                       
                    }
                    catch {
                        Write-Output "Error: $($_.Exception.Message)"
                    }
                }
            }
            Remove {
                Write-Output "Remove User with sAMAccountName $sAMAccountName"

                try {
                    if ($sAMAccountName -ne "") {
                        Get-ADUser -Filter * | where {$_.SamAccountName -eq $sAMAccountName} | Remove-ADUser -Confirm:$false
                    }                    
                    else {
                        Write-Output "Please define a sAMAccountName!"
                    }
                    
                    Write-Output "success"
                }
                catch {
                    Write-Output "Error: $($_.Exception.Message)"
                }
            }
            Default {}
        }
    }
    catch {
        Write-Output "Error: $($_.Exception.Message)"
    }

    #endregion

#endregion
#######################################################################################################################
