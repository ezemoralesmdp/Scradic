﻿using Microsoft.EntityFrameworkCore;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Infrastructure.Data;

namespace Scradic.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _entity;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<User>();
        }

        public async Task RegisterSingleUser(User user)
        {
            _entity.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetSingleUser()
        {
            var query = _entity.AsQueryable().AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }
    }
}