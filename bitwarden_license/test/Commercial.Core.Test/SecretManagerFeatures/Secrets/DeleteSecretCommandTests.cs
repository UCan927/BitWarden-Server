﻿using Bit.Commercial.Core.SecretManagerFeatures.Secrets;
using Bit.Core.Entities;
using Bit.Core.Exceptions;
using Bit.Core.Repositories;
using Bit.Test.Common.AutoFixture;
using Bit.Test.Common.AutoFixture.Attributes;
using NSubstitute;
using Xunit;

namespace Bit.Commercial.Core.Test.SecretManagerFeatures.Secrets;

[SutProviderCustomize]
public class DeleteSecretCommandTests
{
    [Theory]
    [BitAutoData]
    public async Task DeleteSecrets_Throws_NotFoundException(List<Guid> data,
      SutProvider<DeleteSecretCommand> sutProvider)
    {
        sutProvider.GetDependency<ISecretRepository>().GetManyByIds(data).Returns(new List<Secret>());

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => sutProvider.Sut.DeleteSecrets(data));

        //TODO check access level 
        await sutProvider.GetDependency<ISecretRepository>().DidNotReceiveWithAnyArgs().SoftDeleteManyByIdAsync(default);
    }

    [Theory]
    [BitAutoData]
    public async Task DeleteSecrets_OneIdNotFound_Throws_NotFoundException(List<Guid> data,
      SutProvider<DeleteSecretCommand> sutProvider)
    {
        var secret = new Secret()
        {
            Id = Guid.NewGuid()
        };
        sutProvider.GetDependency<ISecretRepository>().GetManyByIds(data).Returns(new List<Secret>() { secret });

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => sutProvider.Sut.DeleteSecrets(data));

        await sutProvider.GetDependency<ISecretRepository>().DidNotReceiveWithAnyArgs().SoftDeleteManyByIdAsync(default);
    }

    [Theory]
    [BitAutoData]
    public async Task DeleteSecrets_Success(List<Guid> data,
      SutProvider<DeleteSecretCommand> sutProvider)
    {
        var secrets = new List<Secret>();
        foreach (Guid id in data)
        {
            var secret = new Secret()
            {
                Id = id
            };
            secrets.Add(secret);
        }

        sutProvider.GetDependency<ISecretRepository>().GetManyByIds(data).Returns(secrets);

        var results = await sutProvider.Sut.DeleteSecrets(data);

        await sutProvider.GetDependency<ISecretRepository>().Received(1).SoftDeleteManyByIdAsync(Arg.Is(data));
        foreach (var result in results)
        {
            Assert.Equal("", result.Item2);
        }
    }
}
