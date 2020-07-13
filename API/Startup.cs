﻿using System;
using System.IO;
using System.Reflection;
using CQRSMediatR.Config;
using CQRSMediatR.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace CQRSMediatR
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("CQRS_SAMPLE");
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddMediatR(typeof(CQRSMediatR.Domain.Commands.Users.CreateUserCommand), typeof(CQRSMediatR.Domain.Queries.Users.GetUserQuery));

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
                });

            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CQRS with MediatR - Example API",
                        Version = "v1",
                        Description = "Simple API built to demonstrate how to use CQRS with MediatR in ASP.NET Core applications.",
                        Contact = new OpenApiContact
                        {
                            Name = "Evandro Gayer Gomes",
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                        },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                cfg.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CQRS with MediatR - Example API v1");
            });

            app.UseMvc();
        }
    }
}