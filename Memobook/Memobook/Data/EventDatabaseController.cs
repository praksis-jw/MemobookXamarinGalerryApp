using Memobook.Data;
using Memobook.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Memobook
{
    public class  
    {
        static object locker = new object();

        SQLiteConnection database;
        public EventDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Event>();
            database.CreateTable<EventUser>();
        }
        public Event GetEvent()
        {
            lock(locker)
            {
                if(database.Table<Event>().Count() ==0)
                { return null; }
                else
                {
                    return database.Table<Event>().First();
                }
            }
        }
        public string SaveEvent(Event eventt)
        {
            lock(locker)
            {
                if(eventt.EventId != "")
                {
                    database.Update(eventt);
                    return eventt.EventId;
                }
                else
                {
                    
                    database.Insert(eventt);
                    database.Commit();
                    return "";
                }
            }

        }

        public Guid SaveUser(EventUser eventuser)
        {
            lock (locker)
            {
                if (eventuser.EventUserID != Guid.Empty)
                {
                    database.Update(eventuser);
                    return eventuser.EventUserID;
                }
                else
                {

                    database.Insert(eventuser);
                    database.Commit();
                    return eventuser.EventUserID;
                }
            }

        }

        public int DeleteEvent(int id)
        {
            lock (locker)
            {
                return database.Delete<Event>(id);
            }
        }
    }
}
