using Microsoft.EntityFrameworkCore;
using sovosTask;
using sovosTask.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;
using System.Data.Common;
using sovosTask.Concrete;
using sovosTask.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();



//builder.Services.AddDbContext<InvoiceDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddDbContext<InvoiceDbContext>(options =>
{
    options.UseSqlServer((builder.Configuration.GetConnectionString("DevConnection")));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});



builder.Services.AddHangfire(x => x.UseSqlServerStorage("Server=.;Database=SovosInvoiceDB;Trusted_Connection=True;MultipleActiveResultSets=true;"));
builder.Services.AddScoped<JobScheduler>(); // JobScheduler'ý ekleyin

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Configuration.Bind("EmailOptions", builder.Configuration.GetSection("EmailOptions"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Invoice}/{action=Index}/{id?}");



// Hangfire'ý yapýlandýrma
app.UseHangfireServer();
app.UseHangfireDashboard();

// JobScheduler'ý baþlatma
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var jobScheduler = services.GetRequiredService<JobScheduler>();
    jobScheduler.ScheduleJob();
}


app.Run();


