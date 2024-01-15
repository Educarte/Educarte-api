using Api.Infrastructure.Services.Interfaces;
using Api.Results.Files;
using FluentValidation;
using MediatR;
using Nudes.Retornator.Core;

namespace Api.Features.Files;

/// <summary>
/// Upload file
/// </summary>
public class Upload
{
    /// <summary>
    /// Upload file command
    /// </summary>
    public class Command : IRequest<ResultOf<FileResult>>
    {
        /// <summary>
        /// Files
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// File object
        /// </summary>
        public class File
        {
            /// <summary>
            /// Name of file
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Extension of file
            /// </summary>
            public string Extension { get; set; }

            /// <summary>
            /// File Stream
            /// </summary>
            public Stream FileStream { get; set; }
        }

    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(d => d.Files).NotEmpty();
            RuleForEach(d => d.Files).ChildRules(op =>
            {
                op.RuleFor(d => d.FileStream).NotNull();
                op.RuleFor(d => d.Extension).NotEmpty();
                op.RuleFor(d => d.Name).NotEmpty();
            });
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<FileResult>>
    {
        private readonly ISpaceService spaceService;

        public Handler(ISpaceService spaceService)
        {
            this.spaceService = spaceService;
        }

        public async Task<ResultOf<FileResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var results = await Task.WhenAll(request.Files.Select(async file =>
            {
                string name = $"{Guid.NewGuid()}{file.Extension}";
                var filePath = await spaceService.UploadFileAsync(file.FileStream, name, cancellationToken);
                return new FileResult.Item
                {
                    Name = file.Name,
                    FilePath = filePath
                };
            }));

            return new FileResult
            {
                Items = results
            };
        }
    }
}
