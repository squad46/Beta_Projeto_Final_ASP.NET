﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Andor.Models;


namespace Andor.Controllers
{
    public class PessoaController : Controller
    {
        private readonly Contexto _context;

        public PessoaController(Contexto context)
        {
            _context = context;
        }



        // GET: Pessoa
        public async Task<IActionResult> Index()
        {
            return NotFound();
            //return View(await _context.Pessoas.ToListAsync());
        }

  

        // GET: Pessoa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.ToString() != Request.Cookies["Id"].ToString())  // Compara se parametro id é diferente de id passado no cookie 
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas.FirstOrDefaultAsync(m => m.Id == id);

            ViewData["formacoes"]    = _context.Formacoes.Where(p => p.Id_pessoa == id).ToList();    // cria lista de formacoes da pessoa
            ViewData["experiencias"] = _context.Experiencias.Where(p => p.Id_pessoa == id).ToList(); // cria lista de experiencias da pessoa
            ViewData["trabalhos"]    = _context.Trabalhos.Where(p => p.Id_pessoa == id).ToList();    // cria lista de trabalhos publicados pela pessoa
            ViewData["moradias"]     = _context.Moradias.Where(p => p.Id_pessoa == id).ToList();     // cria lista de Moradias publicados pela pessoa

            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // GET: Pessoa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pessoa/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Senha,Telefone,CPF,Endereco,Numero,CEP,Sexo,DataNascimento,Nacionalidade,DataCadastro")] Pessoa pessoa)
        {
            //verifica se email já foi cadastrado e retorna mensagem se sim
            var verificaEmail = _context.Pessoas.Where(p => p.Email == pessoa.Email).ToList();
            if (verificaEmail.Count > 0) 
            {
                ViewData["mensagem"]="Este email já foi cadastrado!";
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(pessoa);
                await _context.SaveChangesAsync();
                ViewData["mensagem"] = "Usuário cadastrado com sucesso!";
                return View();
            }
            return View(pessoa);
        }



        // GET: Pessoa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (id.ToString() != Request.Cookies["Id"].ToString())  // Compara se parametro id é diferente de id passado no cookie 
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            return View(pessoa);
            //return Redirect("~/Pessoa/Details/" + id);
        }

        // POST: Pessoa/Edit/5


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Senha,Telefone,CRNM,CPF,Endereco,Numero,CEP,Sexo,DataNascimento,Nacionalidade,DataCadastro")] Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pessoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
  
                return Redirect("~/Pessoa/Details/" + id);
           
            }
           
            return Redirect("~/Pessoa/Details/" + id);
        }

        // GET: Pessoa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.ToString() != Request.Cookies["Id"].ToString())  // Compara se parametro id é diferente de id passado no cookie 
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // POST: Pessoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // deleta moradias anunciadas
            var moradia = _context.Moradias.Where(p => p.Id_pessoa == id).ToList();
            foreach (var _moradia in moradia)
            {
                _context.Moradias.Remove(_moradia);
                await _context.SaveChangesAsync();
            }

            // deleta trabalhos anunciados
            var trabalho = _context.Trabalhos.Where(p => p.Id_pessoa == id).ToList();
            foreach (var _trabalho in trabalho)
            {
                _context.Trabalhos.Remove(_trabalho);
                await _context.SaveChangesAsync();
            }

            // deleta o perfil
            var pessoa = await _context.Pessoas.FindAsync(id);
            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();  

            //cookies excluidos para que seja feito logoff
            Response.Cookies.Delete("Id");
            Response.Cookies.Delete("Usuario");

            return Redirect("~/Home");

        }


        [HttpGet]
        public IActionResult PerfilPublico(int id)
        {
            var pessoa = _context.Pessoas.Where(p => p.Id == id).ToList();
            if (pessoa != null) 
            {
                ViewData["perfil"] = pessoa;
                return View(); 
            }
            return NotFound();
           
        }

        //---------------------------- inicio controles de formacao --------------------------------------------------------------------
        // GET: Formacao/Create
        public IActionResult formacaoCreate(int? Id_pessoa)
        {
            ViewData["Id_pessoa"] = Id_pessoa;
            return View();
        }

        // POST: Formacao/Create - Adiciona formacao profissional no perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> formacaoCreate([Bind("Id,Id_pessoa,Nome,Descricao,Instituicao,Inicio,Fim,Situacao")] Formacao formacao)
        {
            if (ModelState.IsValid)
            {
                var idPessoa = formacao.Id_pessoa;
                _context.Add(formacao);
                await _context.SaveChangesAsync();
                return Redirect("~/Pessoa/Details/" + idPessoa);
            }
            return Redirect("~/Pessoa");
        }


        [HttpPost] // Exclui formacao e retorna ao perfil
        public IActionResult deletarFormacao(Formacao formacao)
        {
            var formacaoDelete = _context.Formacoes.Find(formacao.Id);
            var idPessoa = formacaoDelete.Id_pessoa;
            _context.Remove(formacaoDelete);
            _context.SaveChanges();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }
        //------------------------ fim controles de formacao --------------------------------------------------------------------------


        //------------------------------- inicio controles de experiencia -------------------------------------------------------------

        // GET: Experiencia/Create
        public IActionResult experienciaCreate(int? Id_pessoa)
        {
            ViewData["Id_pessoa"] = Id_pessoa;
            return View();
        }

        // Adiciona experiencia profissional no perfil 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> experienciaCreate([Bind("Id,Id_pessoa,Nome,Descricao,Cargo,Instituicao,Inicio,Fim")] Experiencia experiencia)
        {
            if (ModelState.IsValid)
            {
                var idPessoa = experiencia.Id_pessoa;
                _context.Add(experiencia);
                await _context.SaveChangesAsync();
                return Redirect("~/Pessoa/Details/" + idPessoa);
            }
            return Redirect("~/Pessoa");
        }


        [HttpPost]// Exclui experiencia e retorna para o perfil
        public IActionResult deletarExperiencia(Experiencia experiencia)
        {
            var experienciaDelete = _context.Experiencias.Find(experiencia.Id);
            var idPessoa = experienciaDelete.Id_pessoa;
            _context.Remove(experienciaDelete);
            _context.SaveChanges();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }
        // -------------------------- fim controles de experiencia ---------------------------------------------------------


        //---------------------------- inicio controles de Trabalho --------------------------------------------------------------------
        // GET: Trabalho/Create
        public IActionResult trabalhoCreate(int? Id_pessoa)
        {
            ViewData["Id_pessoa"] = Id_pessoa;
            return View();
        }

        // POST: Trabalho/Create - Adiciona trabalho no perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> trabalhoCreate([Bind("Id,Id_pessoa,Instituicao,Nome,Atividade,Tipo,Salario,Endereco,Numero,CEP,UF,Cidade,NomeContato,TelefoneContato,EmailContato,DataCadastro")] Trabalho trabalho)
        {
            var idPessoa = trabalho.Id_pessoa;
            _context.Add(trabalho);
            await _context.SaveChangesAsync();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }


        [HttpPost] // Exclui oferta de trabalho e retorna ao perfil
        public IActionResult deletarTrabalho(Trabalho trabalho)
        {
            var trabalhoDelete = _context.Trabalhos.Find(trabalho.Id);
            var idPessoa = trabalhoDelete.Id_pessoa;
            _context.Remove(trabalhoDelete);
            _context.SaveChanges();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }
        //------------------------ fim controles de trabalho --------------------------------------------------------------------------


        //------------------------------- inicio controles de moradia -------------------------------------------------------------

        // GET: Moradia/Create
        public IActionResult moradiaCreate(int? Id_pessoa)
        {
            ViewData["Id_pessoa"] = Id_pessoa;
            return View();
        }

        // Adiciona moradia no perfil 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> moradiaCreate([Bind("Id,Id_pessoa,Name,Descricao,Tipo,Preco,Endereco,Numero,CEP,UF,Cidade,NomeContato,TelefoneContato,EmailContato,DataCadastro")] Moradia moradia)
        {
            var idPessoa = moradia.Id_pessoa;
            _context.Add(moradia);
            await _context.SaveChangesAsync();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }


        [HttpPost]// Exclui moradia e retorna para o perfil
        public IActionResult deletarMoradia(Moradia moradia)
        {
            var moradiaDelete = _context.Moradias.Find(moradia.Id);
            var idPessoa = moradiaDelete.Id_pessoa;
            _context.Remove(moradiaDelete);
            _context.SaveChanges();
            return Redirect("~/Pessoa/Details/" + idPessoa);
        }
        // ------------------- fim moradia controles -----------------------

        private bool PessoaExists(int id)
        {
            return _context.Pessoas.Any(e => e.Id == id);
        }
    }
}
