using Api.Infrastructure;
using Api.Results.Users;
using MediatR;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Details of logged user
/// </summary>
public class Me
{
    /// <summary>
    /// Details of logged user query
    /// </summary>
    public class Query : IRequest<ResultOf<UserSimpleResult>>
    {
    }

    internal class Handler : IRequestHandler<Query, ResultOf<UserSimpleResult>>
    {
        private readonly IActor actor;
        private readonly IMediator mediator;

        public Handler(IActor actor, IMediator mediator)
        {
            this.actor = actor;
            this.mediator = mediator;
        }

        public Task<ResultOf<UserSimpleResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            return mediator.Send(new Detail.Query { Id = actor.UserId }, cancellationToken);
        }
    }
}
