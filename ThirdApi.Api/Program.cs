using Blog.Api.Configurations.Mapping;
using Blog.Api.ExceptionHandling;
using Blog.Api.Models.Validators;
using Blog.Api.Persistence;
using Blog.Api.Persistence.Repository.CommentRepo; // New
using Blog.Api.Persistence.Repository.PostRepo;
using Blog.Api.Services.CommentServices; // New
using Blog.Api.Services.PostServices;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;

// Using Top-Level Statements (modern C# convention)
var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------
// 1. Database Context Configuration (MySQL + Entity Framework Core)
// ----------------------------------------------------------------------
/*builder.Services.AddSqlServer<BlogApiContext>(builder.Configuration.GetConnectionString("BlogApiContext"));*/

builder.Services.AddDbContext<BlogApiContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("BlogApiContext")));



// ----------------------------------------------------------------------
// 2. Dependency Injection: Register Repositories (Scoped Lifetime)
// ----------------------------------------------------------------------
builder.Services.AddScoped<IPostRepository, PostRepository>(); 

builder.Services.AddScoped<ICommentRepository, CommentRepository>(); 


// ----------------------------------------------------------------------
// 3. Dependency Injection: Register Services (Scoped Lifetime)
// ----------------------------------------------------------------------
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddScoped<ICommentService, CommentService>();


// ----------------------------------------------------------------------
// 4. API, Controller Setup and Fluent Validation Configuration
// ----------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation(); // Enable automatic validation    
builder.Services.AddFluentValidationClientsideAdapters();  // Enable client-side adapters (if needed)
builder.Services.AddValidatorsFromAssemblyContaining<CommentRequestDtoValidator>(); // Register validators from the specified assembly
// Mapster: register configuration via IRegister implementations (MapsterConfig) discovered automatically.
builder.Services.AddMapster();    // <--- REGISTER MAPSTER SERVICES
builder.Services.AddProblemDetails(); // Optional: Add ProblemDetails middleware for better error responses
builder.Services.AddExceptionHandlingServices(); // Register the custom exception handling services

var app = builder.Build();



// ----------------------------------------------------------------------
// 5. HTTP Request Pipeline Configuration
// ----------------------------------------------------------------------
app.UseCustomExceptionHandler(builder.Environment); // Use the custom exception handling middleware

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();