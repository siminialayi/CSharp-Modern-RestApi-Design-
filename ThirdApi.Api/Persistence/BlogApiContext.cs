using Blog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Persistence;

public class BlogApiContext(DbContextOptions<BlogApiContext> options) : DbContext(options)
    {
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    }








/*
public class BlogApiContext(DbContextOptions<BlogApiContext> options) : DbContext(options)
    {
    public DbSet<Post> Posts => Set<Post>();
    
    }
*/