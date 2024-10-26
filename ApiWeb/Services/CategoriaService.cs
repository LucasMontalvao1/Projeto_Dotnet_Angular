using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            try
            {
                return await _categoriaRepository.GetCategorias();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter categorias: {ex.Message}");
            }
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            try
            {
                return await _categoriaRepository.GetCategoriaById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter categoria: {ex.Message}");
            }
        }

        public async Task<int> AddCategoria(Categoria categoria)
        {
            try
            {
                return await _categoriaRepository.AddCategoria(categoria); // Retorna o ID gerado
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar categoria: {ex.Message}", ex);
            }
        }

        public async Task UpdateCategoria(Categoria categoria)
        {
            try
            {
                await _categoriaRepository.UpdateCategoria(categoria);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar categoria: {ex.Message}");
            }
        }

        public async Task DeleteCategoria(int id)
        {
            try
            {
                await _categoriaRepository.DeleteCategoria(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar categoria: {ex.Message}");
            }
        }
    }
}
