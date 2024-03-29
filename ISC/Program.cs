using AutoMapper;
using ISC.EF;
using ISC.Services.Helpers;
using ISC.Services.ISerivces;
using ISC.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;

namespace ISC
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.
			builder.Services.addServices();
			//builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
			builder.Services.AddDbContext<DataBase>(option =>
				option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
									b => b.MigrationsAssembly(typeof(DataBase).Assembly.FullName))
									);
			builder.Services.AddIdentity<UserAccount, IdentityRole>()
				.AddEntityFrameworkStores<DataBase>()
				.AddDefaultTokenProviders();
			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
			builder.Services.Configure<DefaultMessages>(builder.Configuration.GetSection("DefaultMessages"));
			builder.Services.Configure<CodeForceConnection>(builder.Configuration.GetSection("CodeForceConnection"));
			builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
			
			// Auto Mapper Configuration
			var mapperConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new AutoMapperProfile());
			});
			IMapper mapper = mapperConfig.CreateMapper();
			builder.Services.AddSingleton(mapper);


			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(o =>
			{
				o.RequireHttpsMetadata = false;
				o.SaveToken = false;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidIssuer = builder.Configuration["JWT:Issuer"],
					ValidAudience = builder.Configuration["JWT:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
					ClockSkew = TimeSpan.Zero
				};
			});
			builder.Services.AddControllers();
			//builder.Services.AddInfrastructure();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddCors();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });
			});
			builder.Services.AddSwaggerGen(swagger =>
			{
				//This is to generate the Default UI of Swagger Documentation    
				swagger.SwaggerDoc("v2", new OpenApiInfo
				{
					Version = "v1",
					Title = "ASP.NET 5 Web API",
					Description = " ITI Projrcy"
				});

				// To Enable authorization using Swagger (JWT)    
				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
				});
				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
					new OpenApiSecurityScheme
					{
					Reference = new OpenApiReference
					{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
					}
					},
					new string[] {}
					}
				});
			});
			var app = builder.Build();
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			Seed(app);
			
			app.Run();
		}
		static  void Seed(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var Initalizer = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
			Initalizer.Seeding().Wait();
		}
	}
}