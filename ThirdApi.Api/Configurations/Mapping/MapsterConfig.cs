using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;
using Mapster;

namespace Blog.Api.Configurations.Mapping
    {
    /// <summary>
    /// Mapster configuration implementing IRegister so that AddMapster() picks it up automatically.
    /// Keeps mappings scoped to the DI-provided TypeAdapterConfig instead of relying on GlobalSettings.
    /// </summary>
    public class MapsterConfig : IRegister
        {
        public void Register(TypeAdapterConfig config)
            {
            // 1. CommentRequestDto -> Comment (incoming write operations)
            config.NewConfig<CommentRequestDto, Comment>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Author); // Author is provided by service/auth context.

            // 2. Comment -> CommentResponseDto (outgoing read operations)
            config.NewConfig<Comment, CommentResponseDto>()
                .MaxDepth(2);

            // (Add more mappings here as the domain grows: Post, User, etc.)
            }
        }
    }