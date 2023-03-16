using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Domain.AggregatesModel.DemoAggregate;
using ConsoleApp.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Infrastructure.Repositories
{
    internal class DemoRepository : IDemoRepository
    {
        private readonly DemoContext _context;

        public DemoRepository(DemoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public Demo Add(Demo demo)
        {
            return _context.Demos.Add(demo).Entity;
        }

        public void Update(Demo demo)
        {
            _context.Entry(demo).State = EntityState.Modified;
        }

        public async Task<Demo> GetAsync(int demoId)
        {
            var demo = await _context
                .Demos
                .FirstOrDefaultAsync(o => o.Id == demoId);
            if (demo == null)
            {
                demo = _context
                    .Demos
                    .Local
                    .FirstOrDefault(o => o.Id == demoId);
            }

            return demo;
        }
    }
}
