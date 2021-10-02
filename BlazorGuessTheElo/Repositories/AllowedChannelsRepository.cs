using BlazorGuessTheElo.DataContext;
using BlazorGuessTheElo.Models;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services;
using BlazorGuessTheElo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Repositories
{
    public class AllowedChannelsRepository : IAllowedChannelsRepository
    {
        private EloSubmissionDatabaseContext databaseContext;
        private IDatabaseChangesService databaseChangesService;
        public AllowedChannelsRepository(EloSubmissionDatabaseContext databaseContext, IDatabaseChangesService databaseChangesService)
        {

            this.databaseContext = databaseContext;
            this.databaseChangesService = databaseChangesService;
            //this.databaseContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<EloSubmissionDatabaseContext>();
            //this.databaseContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void AddAllowedChannel(ulong channelId)
        {
            var channel = databaseContext.AllowedChannels.FirstOrDefault(x => x.ChannelId == channelId);
            if (channel == null)
            {
                channel = new AllowedChannel(channelId);
                channel.Allowed = true;
                databaseContext.AllowedChannels.Add(channel);
            }
            else
            {
                channel.Allowed = true;
                databaseContext.Update(channel);
            }
            databaseContext.SaveChanges();
            databaseChangesService.RefreshChannelAdded(channelId);
        }



        public List<ulong> GetAllowedChannels()
        {
            return databaseContext.AllowedChannels.AsQueryable().Where(x => x.Allowed).Select(x => x.ChannelId).ToList();
        }

        public void RemoveAllowedChannel(ulong channelId)
        {
            AllowedChannel channel = databaseContext.AllowedChannels.FirstOrDefault(x => x.ChannelId == channelId);
            channel.Allowed = false;
            databaseContext.AllowedChannels.Update(channel);
            databaseContext.SaveChanges();
            databaseChangesService.RefreshChannelRemoved(channelId);
        }
        public ulong? GetDefaultRoleByChannelId(ulong channelId)
        {
            ulong? role = databaseContext.AllowedChannels.FirstOrDefault(x => x.ChannelId == channelId).DefaultRole;
            return role;
        }

        public void SetDefaultRoleByChannelId(ulong channelId, ulong roleId)
        {
            var allowedChannel = databaseContext.AllowedChannels.FirstOrDefault(x => x.ChannelId == channelId);
            allowedChannel.Allowed = true;
            allowedChannel.DefaultRole = roleId;
            databaseContext.Update(allowedChannel);
            databaseContext.SaveChanges();
        }
    }
}
