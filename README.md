# ASP.NET Core with On-Premises Active Directory Authentication

Starter kit to quickly create ASP.NET Core 2.0 with On-Premises Active Directory Authentication.

### Requirements
  * [ASP.NET Core SDK 2.0](https://www.microsoft.com/net/download/core)
  * Visual Studio 2017
  * Telerik Kendo UI for AspNet Core 2017.3.913
  
### Options  
  * [BundlerMinifier](https://github.com/madskristensen/BundlerMinifier)

Database script is located at [Tables.sql](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/Tables.sql).

## Database Diagram
![Database Diagram](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/DatabaseDiagram.png "Database Diagram")

## User Login
![User Login](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/Login.png "User Login")

## User Dashboard
![User Dashboard](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/UserDashboard.png "User Dashboard")

## Admin Dashboard
![Admin Dashboard](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminDashboard.png "Admin Dashboard")

## Admin Application Logs
![Admin Application Logs](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminApplicationLogs.png "Admin Application Logs")

## Admin Application Log Detail
![Admin Application Log Detail](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminApplicationLogDetail.png "Admin Application Log Detail")

## Admin Email Templates
![Admin Email Templates](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminEmailTemplateList.png "Admin Email Templates")

## Admin Email Template Edit
![Admin Dashboard](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminEmailTemplateEdit.png "Admin Email Template Edit")

## Admin User List
![Admin User List](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminUserList.png "Admin User List")

## Admin User Create
![Admin User Create](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminUserCreate.png "Admin User Create")

## Admin Settings
![Admin Settings](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminSettings.png "Admin Settings")

## Admin Setting Edit
![Admin Setting Edit](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/AdminSettingEdit.png "Admin Setting Edit")

## Session Expire Notification
![Session Expire Notification](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/SessionExpireNotification.png "Session Expire Notification")

## Toaster
Toaster will disappear after 10 seconds. You can configure it at [site.js](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/src/Presentation/Asp.Web/wwwroot/js/site.js#L62).

![Toaster](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/Toaster.png "Toaster")

## Troubleshooting
### Offline Nuget Packages
The application uses Telerik UI for ASP.NET Core that allows using the Kendo UI widgets from C# server-side wrappers. 

Download Trial version of Kendo UI from [Telerik](http://www.telerik.com), and add Kendo UI as OfflineNugetPackages by following the instruction below. *Note: I am not affiliated with neither Telerik or Kendo UI.*

Extract `Telerik.UI.for.AspNet.Core.xxx.nupkg` from the downloaded Zip file to a local folder. For example, `C:\OfflineNuGetPackages\telerik.ui.for.aspnet.core`

![Offline Nuget Packages Location](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/OfflineNugetPackagesLocation.png "Offline Nuget Packages Location")

In Visual Studio, go to `Tools > Options > NuGet Package Manager > Package Sources`, and add a new package source.

![Offline Nuget Configuration](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/OfflineNugetPackagesConfiguration.png "Offline Nuget Configuration")

Then you can select `OfflineNuGetPackages` from Package source, select projects, and click Install.

![Offline Nuget Packages](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/OfflineNugetPackages.png "Offline Nuget Packages")

### ASP.NET Core
Please make sure you have correct ASP.NET Core version under `C:\Program Files\dotnet\sdk`

![ASP.NET Core](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/ASPNetCore.png "ASP.NET Core")

## Testing 
### Debug without Active Directory
If you would like to view the demo of the application without Active Directory, you could use the following steps -

Create database tables in SQL Server using the provided sql script inside doc folder. Replace the connection string inside `appsettings.json`.

Return `true` value at `if statement` of `Account Controller` line 71. At Login Screen, enter `johndoe` and anything in the password.

*Note: Please make sure to revert those changes after you've done the demo.*

![Debug without Active Directory](https://github.com/WinLwinOoNet/AspNetCoreActiveDirectoryStarterKit/blob/master/doc/screenshots/DebugWithoutAD.png "Debug without Active Directory")
