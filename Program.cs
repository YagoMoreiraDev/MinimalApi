using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Aqui vou criar meus endpoints;
//Função qeu retorna uma lista atualizada de usuarios;
async Task<List<UsuarioModel>> GetUsuarios(AppDbContext context)
{
    return await context.Usuarios.ToListAsync();
}

//1º Pegar todos os usuarios do banco;
app.MapGet("/usuarios", async (AppDbContext context) =>
{
    return await context.Usuarios.ToListAsync();
});

//3º Buscar um usuario
app.MapGet("/usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);

    if (usuario == null) return Results.NotFound("Usuário não localizado!");

    return Results.Ok(usuario);
});

//2º Criação de usuarios;
app.MapPost("/usuario", async (AppDbContext context, UsuarioModel user) =>
{
    context.Usuarios.Add(user);
    await context.SaveChangesAsync();

    return await GetUsuarios(context);
});

//4º Alterando um usuario
app.MapPut("/usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    var usuarioDb = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(usuarioDb => usuarioDb.Id == usuario.Id);

    if (usuarioDb == null) return Results.NotFound("Usuário não localizado!");

    usuarioDb.Username = usuario.Username;
    usuarioDb.Email = usuario.Email;
    usuarioDb.Nome = usuario.Nome;

    context.Update(usuario);

    await context.SaveChangesAsync();

    return Results.Ok(await GetUsuarios(context));

});

//5º Deletando usuario
app.MapDelete("/usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuarioDb = await context.Usuarios.FindAsync(id);

    if (usuarioDb == null) return Results.NotFound("Usuário não localizado!");

    context.Usuarios.Remove(usuarioDb);
    await context.SaveChangesAsync();

    return Results.Ok(await GetUsuarios(context));

});

app.Run();
