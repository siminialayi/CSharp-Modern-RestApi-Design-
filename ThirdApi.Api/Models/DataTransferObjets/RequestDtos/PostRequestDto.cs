using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Models.DataTransferObjets.RequestDtos;
public class PostRequestDto
    {
    
    public string Title { get; set; } = string.Empty;

   
    public string Content { get; set; } = string.Empty;
    }