# Booking API

Simple Book API. Using monitoring, logs, health checks and error tracking.

<h3>Tools</h3>
<ul>
    <li>VS2022 (optional)</li>
    <li>.NET Core 6.0</li>
    <li><a href="https://serilog.net/" target="_blank">Serilog</a></li>
    <liSwagger UI</li>
    <li><a href="https://prometheus.io/" target="_blank">Prometheus</a></li>
    <li><a href="https://grafana.com/" target="_blank">Grafana Dashboard</a></li>
    <li><a href="https://sentry.io/" target="_blank">Sentry</a></li>
    <li>Microsoft Entity Framework Core</li>
    <li><a href="https://www.microsoft.com/en-us/sql-server/sql-server-2019" target="_blank">Microsoft SQL Server</a></li>
</ul>

<h3>How to run</h3>
<ol>
    <li>Restore all packages</li>
    <li>Change the ConnectionString in <i>appsettings.json</i></li>
    <li>Change Sentry configurations in <i>appsettings.json</i></li>
    <li>Apply migrations <code>Add-Migration {migration name}</code></li>
    <li>Apply migrations changes <code>Update-Database</code></li>
</ol>

Configure prometheus in <code>prometheus.yml</code> adding the following lines:

```yml
- job_name: 'bookingapi' 
  static_configs: 
    - targets: ["<host>:<port>"] 
  metrics_path: "/metrics-text" 
  scheme: "<http><https>"
  ```

![Grafana Dashboard Image](https://github.com/JMatoso/Booking/blob/30309a820b193fad06d0e51900531bd7e476b161/project-files/dashboard-print.png?raw=true)

Grafana Dashboard in <a href="https://github.com/JMatoso/Booking/blob/8b57188b40c8a1dbde3c9fff77ef3c1a78d0701e/project-files/booking-dashboard.json"><code>project-files</code></a> folder.
