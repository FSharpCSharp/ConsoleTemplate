using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Application.Commands
{
    internal class CreateDemoCommand : IRequest<bool>
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        public CreateDemoCommand()
        {

        }

        public CreateDemoCommand(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
