﻿using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AspNetCoreRateLimit
{
    public class DistributedCacheClientPolicyStore(
        IDistributedCache cache,
        IOptions<ClientRateLimitOptions> options = null,
        IOptions<ClientRateLimitPolicies> policies = null) : DistributedCacheRateLimitStore<ClientRateLimitPolicy>(cache), IClientPolicyStore
    {
        private readonly ClientRateLimitOptions _options = options?.Value;
        private readonly ClientRateLimitPolicies _policies = policies?.Value;

        public async Task SeedAsync()
        {
            // on startup, save the IP rules defined in appsettings
            if (_options != null && _policies?.ClientRules != null)
            {
                foreach (var rule in _policies.ClientRules)
                {
                    await SetAsync($"{_options.ClientPolicyPrefix}_{rule.ClientId}", new ClientRateLimitPolicy { ClientId = rule.ClientId, Rules = rule.Rules }).ConfigureAwait(false);
                }
            }
        }
    }
}