# Booking API

Simple Book API. Using monitoring and health checks.

<h3>Tools</h3>
<ul>
    <li>VS 2022 (optional)</li>
    <li>.NET Core 6.0</li>
    <li>Microsoft Entity Framework Core</li>
    <li> <a href="https://www.microsoft.com/en-us/sql-server/sql-server-2019" target="_blank">Microsoft SQL Server</a></li>
</ul>

<h3>How to run</h3>
<ol>
    <li>Restore all packages</li>
    <li>Change the ConnectionString in <i>appsettings.json</i></li>
    <li>Apply migrations <code>Add-Migration {migration name}</code></li>
    <li>Apply migrations changes <code>Update-Database</code></li>
</ol>

Prometheus in <code>prometheus.yml</code> add the following line:

<code>- job_name: 'bookingapi' <br>
  static_configs:<br>
    - targets: ["localhost:7197"]<br>
  metrics_path: "/metrics-text"<br>
  scheme: "https"</code><br>

