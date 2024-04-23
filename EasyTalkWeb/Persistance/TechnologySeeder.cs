using EasyTalkWeb.Models;

namespace EasyTalkWeb.Persistance.Seeders
{
    public class TechnologySeeder
    {
        public static async Task SeedTechnologiesAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (context.Technologies.Any())
                {
                    return;
                }

                await SeedTechnologies(context);
            }
        }

        private static async Task SeedTechnologies(AppDbContext context)
        {
            var technologies = new[]
            {
                new Technology { Id = Guid.NewGuid(), Name = "UI/UX Design" },
                new Technology { Id = Guid.NewGuid(), Name = "Web Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Frontend Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Backend Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Full-Stack Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Mobile Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Cross-Platform Development" },
                new Technology { Id = Guid.NewGuid(), Name = "Responsive Design" },
                new Technology { Id = Guid.NewGuid(), Name = "Progressive Web Apps" },
                new Technology { Id = Guid.NewGuid(), Name = "Single Page Applications" },
                new Technology { Id = Guid.NewGuid(), Name = "Microservices Architecture" },
                new Technology { Id = Guid.NewGuid(), Name = "Serverless Architecture" },
                new Technology { Id = Guid.NewGuid(), Name = "Containerization" },
                new Technology { Id = Guid.NewGuid(), Name = "DevOps" },
                new Technology { Id = Guid.NewGuid(), Name = "Agile Methodologies" },
                new Technology { Id = Guid.NewGuid(), Name = "Scrum" },
                new Technology { Id = Guid.NewGuid(), Name = "Kanban" },
                new Technology { Id = Guid.NewGuid(), Name = "Continuous Integration" },
                new Technology { Id = Guid.NewGuid(), Name = "Continuous Deployment" },
                new Technology { Id = Guid.NewGuid(), Name = "Version Control" },
                new Technology { Id = Guid.NewGuid(), Name = "Software Testing" },
                new Technology { Id = Guid.NewGuid(), Name = "User Interface Design" },
                new Technology { Id = Guid.NewGuid(), Name = "User Experience Design" },
                new Technology { Id = Guid.NewGuid(), Name = "Wireframing" },
                new Technology { Id = Guid.NewGuid(), Name = "Prototyping" },
                new Technology { Id = Guid.NewGuid(), Name = "Usability Testing" },
                new Technology { Id = Guid.NewGuid(), Name = "Accessibility" },
                new Technology { Id = Guid.NewGuid(), Name = "Responsive Web Design" },
                new Technology { Id = Guid.NewGuid(), Name = "CSS Frameworks" },
                new Technology { Id = Guid.NewGuid(), Name = "CSS Preprocessors" },
                new Technology { Id = Guid.NewGuid(), Name = "CSS-in-JS" },
                new Technology { Id = Guid.NewGuid(), Name = "HTML5" },
                new Technology { Id = Guid.NewGuid(), Name = "Semantic HTML" },
                new Technology { Id = Guid.NewGuid(), Name = "JavaScript Frameworks" },
                new Technology { Id = Guid.NewGuid(), Name = "Frontend Frameworks" },
                new Technology { Id = Guid.NewGuid(), Name = "React.js" },
                new Technology { Id = Guid.NewGuid(), Name = "Angular" },
                new Technology { Id = Guid.NewGuid(), Name = "Vue.js" },
                new Technology { Id = Guid.NewGuid(), Name = "State Management Libraries" },
                new Technology { Id = Guid.NewGuid(), Name = "Redux" },
                new Technology { Id = Guid.NewGuid(), Name = "MobX" },
                new Technology { Id = Guid.NewGuid(), Name = "Frontend Build Tools" },
                new Technology { Id = Guid.NewGuid(), Name = "Webpack" },
                new Technology { Id = Guid.NewGuid(), Name = "Parcel" },
                new Technology { Id = Guid.NewGuid(), Name = "Rollup" },
                new Technology { Id = Guid.NewGuid(), Name = "Frontend Testing Frameworks" },
                new Technology { Id = Guid.NewGuid(), Name = "Jest" },
                new Technology { Id = Guid.NewGuid(), Name = "Mocha" },
                new Technology { Id = Guid.NewGuid(), Name = "Chai" },
                new Technology { Id = Guid.NewGuid(), Name = "Cypress" },
                new Technology { Id = Guid.NewGuid(), Name = "End-to-End Testing" },
                new Technology { Id = Guid.NewGuid(), Name = "Unit Testing" },
                new Technology { Id = Guid.NewGuid(), Name = "Integration Testing" },
                new Technology { Id = Guid.NewGuid(), Name = "Web APIs" },
                new Technology { Id = Guid.NewGuid(), Name = "RESTful APIs" },
                new Technology { Id = Guid.NewGuid(), Name = "GraphQL" },
                new Technology { Id = Guid.NewGuid(), Name = "AJAX" },
                new Technology { Id = Guid.NewGuid(), Name = "Frontend Performance Optimization" },
                new Technology { Id = Guid.NewGuid(), Name = "Code Splitting" },
                new Technology { Id = Guid.NewGuid(), Name = "Lazy Loading" },
                new Technology { Id = Guid.NewGuid(), Name = "Tree Shaking" },
                new Technology { Id = Guid.NewGuid(), Name = "Caching Strategies" },
                new Technology { Id = Guid.NewGuid(), Name = "Service Workers" },
                new Technology { Id = Guid.NewGuid(), Name = "WebAssembly" },
                new Technology { Id = Guid.NewGuid(), Name = "Progressive Enhancement" },
                new Technology { Id = Guid.NewGuid(), Name = "C++ Development" },
                new Technology { Id = Guid.NewGuid(), Name = ".NET Framework" },
                new Technology { Id = Guid.NewGuid(), Name = "ASP.NET" },
                new Technology { Id = Guid.NewGuid(), Name = "ASP.NET Core" },
                new Technology { Id = Guid.NewGuid(), Name = "MVC Framework" },
                new Technology { Id = Guid.NewGuid(), Name = "Web Forms" },
                new Technology { Id = Guid.NewGuid(), Name = "Web API" },
                new Technology { Id = Guid.NewGuid(), Name = "Entity Framework" },
                new Technology { Id = Guid.NewGuid(), Name = "LINQ" },
                new Technology { Id = Guid.NewGuid(), Name = "Razor Pages" }
            };

            await context.Technologies.AddRangeAsync(technologies);
            await context.SaveChangesAsync();
        }
    }
}
