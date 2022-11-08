using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _imapper;

        public MessageRepository(DataContext context,IMapper imapper)
        {
            _context = context;
            _imapper = imapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
            
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessages(int id)
        {
            return await _context.Messages
                .Include(u=>u.Sender)
                .Include(u=>u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username 
                && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username 
                && u.SenderDeleted == false),
                _ =>query.Where(u=> u.Recipient.UserName == messageParams.Username 
                && u.RecipientDeleted ==false && u.DateRead==null)
            };
            var messages = query.ProjectTo<MessageDTO>(_imapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber
                , messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(p=>p.Sender).ThenInclude(l=>l.Photos)
                .Include(p => p.Recipient).ThenInclude(l => l.Photos)
                .Where(x => x.RecipientUsername == currentUsername && x.RecipientDeleted==false
                && x.SenderUsername == recipientUsername
                || x.RecipientUsername == recipientUsername
                && x.SenderUsername == currentUsername && x.SenderDeleted==false
                ).OrderBy(x=>x.MessageSent)
                .ToListAsync();
            var unreadMessages = messages.Where(x => x.DateRead == null &&
            x.RecipientUsername == currentUsername);

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
            return _imapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }
    }
}
