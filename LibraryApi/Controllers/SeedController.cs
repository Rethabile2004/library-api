using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("authors")]
        public async Task<ActionResult> SeedAuthors()
        {
            if (_context.Authors.Any())
            {
                return BadRequest(new { message = "Authors already seeded." });
            }

            var authors = new List<Author>
            {
                new Author
                {
                    Name = "George Orwell",
                    Bio = "Eric Arthur Blair, known by his pen name George Orwell, was an English novelist and essayist. His work is marked by lucid prose, social criticism, and opposition to totalitarianism. He is best known for the allegorical novella Animal Farm and the dystopian novel Nineteen Eighty-Four."
                },
                new Author
                {
                    Name = "Fyodor Dostoevsky",
                    Bio = "Fyodor Mikhailovich Dostoevsky was a Russian novelist and philosopher whose works explore human psychology in the troubled political, social, and spiritual atmosphere of 19th-century Russia. His most celebrated novels include Crime and Punishment, The Idiot, and The Brothers Karamazov."
                },
                new Author
                {
                    Name = "Chimamanda Ngozi Adichie",
                    Bio = "Chimamanda Ngozi Adichie is a Nigerian author widely regarded as one of the most important writers of her generation. Her work spans novels, short stories, and nonfiction, exploring themes of identity, feminism, and the African experience. She is best known for Half of a Yellow Sun and Americanah."
                },
                new Author
                {
                    Name = "Gabriel García Márquez",
                    Bio = "Gabriel José de la Concordia García Márquez was a Colombian novelist and Nobel Prize laureate widely considered one of the greatest writers in the Spanish language. He pioneered the literary style known as magical realism, most famously in One Hundred Years of Solitude."
                },
                new Author
                {
                    Name = "Toni Morrison",
                    Bio = "Toni Morrison was an American novelist, essayist, and professor who won the Nobel Prize in Literature in 1993. Her work is known for its epic themes, vivid dialogue, and richly detailed African American characters. Her most celebrated novel, Beloved, explores the trauma of slavery in post-Civil War America."
                },
                new Author
                {
                    Name = "Franz Kafka",
                    Bio = "Franz Kafka was a German-speaking Bohemian novelist whose work, largely published posthumously, is characterized by anxiety, existential dread, and surreal bureaucratic nightmares. His novels The Trial and The Metamorphosis have given rise to the term Kafkaesque, used to describe absurd and oppressive situations."
                },
                new Author
                {
                    Name = "Haruki Murakami",
                    Bio = "Haruki Murakami is a Japanese writer whose novels blend the mundane with the surreal, often featuring themes of loneliness, music, and parallel realities. His work has been translated into over fifty languages and has earned him numerous international literary awards. Norwegian Wood and Kafka on the Shore are among his most beloved works."
                },
                new Author
                {
                    Name = "Virginia Woolf",
                    Bio = "Virginia Woolf was an English writer considered one of the most important modernist authors of the twentieth century. She pioneered the use of stream of consciousness as a narrative device, most notably in Mrs Dalloway and To the Lighthouse. She was also a prolific essayist and a central figure in the Bloomsbury Group."
                },
                new Author
                {
                    Name = "Chinua Achebe",
                    Bio = "Chinua Achebe was a Nigerian novelist, poet, and critic widely regarded as the father of modern African literature. His debut novel Things Fall Apart is the most widely read book in modern African literature and has been translated into more than fifty languages. His work challenged colonial narratives and gave voice to the African experience."
                },
                new Author
                {
                    Name = "Leo Tolstoy",
                    Bio = "Count Lev Nikolayevich Tolstoy was a Russian novelist widely regarded as one of the greatest authors of all time. His masterworks War and Peace and Anna Karenina are considered the pinnacle of realist fiction. Later in life he became a moral philosopher and spiritual thinker, influencing figures such as Gandhi and Martin Luther King Jr."
                }
            };

            await _context.Authors.AddRangeAsync(authors);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{authors.Count} authors seeded successfully." });
        }

        [HttpGet("books")]
        public async Task<ActionResult> SeedBooks()
        {
            if (_context.Books.Any())
            {
                var count = await _context.Books.CountAsync();

                return BadRequest(new { message = "Books already seeded.", count = count });
            }

            var books = new List<Book>
    {
        new Book { Title = "Nineteen Eighty-Four", ISBN = "9780451524935", PublishedYear = 1949, Genre = "Dystopian", AuthorId = 1 },
        new Book { Title = "Animal Farm", ISBN = "9780451526342", PublishedYear = 1945, Genre = "Political Satire", AuthorId = 1 },
        new Book { Title = "Crime and Punishment", ISBN = "9780143107637", PublishedYear = 1866, Genre = "Psychological Fiction", AuthorId = 2 },
        new Book { Title = "The Brothers Karamazov", ISBN = "9780374528379", PublishedYear = 1880, Genre = "Philosophical Fiction", AuthorId = 2 },
        new Book { Title = "Half of a Yellow Sun", ISBN = "9781400095209", PublishedYear = 2006, Genre = "Historical Fiction", AuthorId = 3 },
        new Book { Title = "Americanah", ISBN = "9780307455925", PublishedYear = 2013, Genre = "Contemporary Fiction", AuthorId = 3 },
        new Book { Title = "One Hundred Years of Solitude", ISBN = "9780060883287", PublishedYear = 1967, Genre = "Magical Realism", AuthorId = 4 },
        new Book { Title = "Love in the Time of Cholera", ISBN = "9780307389732", PublishedYear = 1985, Genre = "Romance", AuthorId = 4 },
        new Book { Title = "Beloved", ISBN = "9781400033416", PublishedYear = 1987, Genre = "Historical Fiction", AuthorId = 5 },
        new Book { Title = "Song of Solomon", ISBN = "9781400033423", PublishedYear = 1977, Genre = "Literary Fiction", AuthorId = 5 },
        new Book { Title = "The Trial", ISBN = "9780805209990", PublishedYear = 1925, Genre = "Absurdist Fiction", AuthorId = 6 },
        new Book { Title = "The Metamorphosis", ISBN = "9780553213690", PublishedYear = 1915, Genre = "Absurdist Fiction", AuthorId = 6 },
        new Book { Title = "Norwegian Wood", ISBN = "9780375704024", PublishedYear = 1987, Genre = "Literary Fiction", AuthorId = 7 },
        new Book { Title = "Kafka on the Shore", ISBN = "9781400079278", PublishedYear = 2002, Genre = "Magical Realism", AuthorId = 7 },
        new Book { Title = "Mrs Dalloway", ISBN = "9780156628709", PublishedYear = 1925, Genre = "Modernist Fiction", AuthorId = 8 },
        new Book { Title = "To the Lighthouse", ISBN = "9780156907392", PublishedYear = 1927, Genre = "Modernist Fiction", AuthorId = 8 },
        new Book { Title = "Things Fall Apart", ISBN = "9780385474542", PublishedYear = 1958, Genre = "Literary Fiction", AuthorId = 9 },
        new Book { Title = "Arrow of God", ISBN = "9780385014809", PublishedYear = 1964, Genre = "Literary Fiction", AuthorId = 9 },
        new Book { Title = "War and Peace", ISBN = "9781400079988", PublishedYear = 1869, Genre = "Historical Fiction", AuthorId = 10 },
        new Book { Title = "Anna Karenina", ISBN = "9780143035008", PublishedYear = 1878, Genre = "Literary Fiction", AuthorId = 10 }
    };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"{books.Count} books seeded successfully." });
        }
    }
}