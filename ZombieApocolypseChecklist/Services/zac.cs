using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.IO;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using ZombieApocolypseChecklist.Data;

namespace ZombieApocolypseChecklist.Services
{
    public class ZAC : NancyModule
    {
        private ChecklistContext context = new ChecklistContext();

        public ZAC() : base("/services/ZAC")
        {
            Get["/Items"] = x => GetItems();
            Get["/Items/{id}"] = x => GetItemById(x.id);
            Post["/Items"] = x => CreateNewItem();
            Put["/Items/{id}"] = x => UpdateItem(x.id);
            Delete["/Items/{id}"] = x => DeleteItem(x.id);
        }

        private Response GetItemById(int id)
        {
            var item = context.Items.First(i => i.id == id);

            return Response.AsJson(item);
        }

        private Response UpdateItem(int id)
        {
            var item = this.Bind<ChecklistItem>();
            var existingItem = context.Items.First(i => i.id == id);
            existingItem.Content = item.Content;
            existingItem.IsCompleted = item.IsCompleted;
            context.SaveChanges();

            return new Response();
        }

        private Response DeleteItem(int id)
        {
            var item = context.Items.First(i => i.id == id);
            context.Items.Remove(item);
            context.SaveChanges();

            return new Response();
        }

        private Response GetItems()
        {
            var items = context.Items;

            return Response.AsJson(items.ToArray());
        }

        private Response CreateNewItem()
        {
            var item = this.Bind<ChecklistItem>();
            context.Items.Add(item);
            context.SaveChanges();

            return Response.AsJson(item);
        }
    }

    public static class RequestExtensions
    {
        public static T FromJson<T>(this RequestStream body)
        {
            var reader = new StreamReader(body, true);
            body.Position = 0;
            var value = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }
    }
}