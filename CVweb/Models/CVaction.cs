using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Models
{
    public class CVaction
    {
        private readonly DBapp context;
        private object c;

        public CVaction(DBapp context)
        {
            this.context = context;
        }

        public CVaction(DBapp context, object c) : this(context)
        {
            this.c = c;
        }

        public IOrderedQueryable<CVz> getadmin()
        {
            return context.CVzs.Where(s => s.inwork == false).OrderBy(s=> s.id);

        }
        public IEnumerable<CVz> getall()
        {
            return context.CVzs;
        }

        public IEnumerable<CVz> getyours(string id)
        {
           return context.CVzs.Where(s =>s.userID == id);
             
        }

        public CVz add(CVz u)
        {
            context.CVzs.Add(u);
            context.SaveChanges();
            return u;
        }
        public CVz Delete(int id)
        {
            CVz user = context.CVzs.Find(id);
            if (user != null)
            {
                context.CVzs.Remove(user);
                context.SaveChanges();
            }
            return user;

        }

        public CVz get(int id)
        {
            return context.CVzs.Find(id);
        }

        public CVz update(CVz newuser)
        {
            var u = context.CVzs.Attach(newuser);
            u.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return newuser;
        }


    }
}