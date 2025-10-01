using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;
using Mapster;

namespace Blog.Api.Configurations.Mapping
{
    public static class MapsterConfig
    {
        public static void RegisterMapping()
        {
            // ----------------------------------------------------------------------------------
            // 1. CommentRequestDto to Comment Entity (For POST/PUT operations)
            // ----------------------------------------------------------------------------------
            TypeAdapterConfig<CommentRequestDto, Comment>.NewConfig()
                // BaseEntity properties and properties handled by the Service/Auth Layer MUST be ignored
                // when mapping FROM a DTO, as they are not provided by the client.
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Author); // Author is set via the authentication context in the Service layer.

            // ----------------------------------------------------------------------------------
            // 2. Comment Entity to CommentResponseDto (For GET operations)
            // ----------------------------------------------------------------------------------
            TypeAdapterConfig<Comment, CommentResponseDto>.NewConfig()
                // All properties match by name (Id, PostId, Author, Content, CreatedAt)
                // The mapping is automatic (convention-based), requiring no explicit configuration.
                .MaxDepth(2); // Best practice for preventing potential recursion in complex graphs.

            // Scan for other mappings in this assembly
            TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfig).Assembly);
        }
    }
}using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;
using Mapster;

namespace Blog.Api.Configurations.Mapping
{
    public static class MapsterConfig
    {
        public static void RegisterMapping()
        {
            // ----------------------------------------------------------------------------------
            // 1. CommentRequestDto to Comment Entity (For POST/PUT operations)
            // ----------------------------------------------------------------------------------
            TypeAdapterConfig<CommentRequestDto, Comment>.NewConfig()
                // BaseEntity properties and properties handled by the Service/Auth Layer MUST be ignored
                // when mapping FROM a DTO, as they are not provided by the client.
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Author); // Author is set via the authentication context in the Service layer.

            // ----------------------------------------------------------------------------------
            // 2. Comment Entity to CommentResponseDto (For GET operations)
            // ----------------------------------------------------------------------------------
            TypeAdapterConfig<Comment, CommentResponseDto>.NewConfig()
                // All properties match by name (Id, PostId, Author, Content, CreatedAt)
                // The mapping is automatic (convention-based), requiring no explicit configuration.
                .MaxDepth(2); // Best practice for preventing potential recursion in complex graphs.

            // Scan for other mappings in this assembly
            TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfig).Assembly);
        }
    }
}