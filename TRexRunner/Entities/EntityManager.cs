﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRexRunner.Entities
{
    public class EntityManager
    {
        private readonly List<IGameEntity> _entities = new List<IGameEntity>();
        
        private readonly List<IGameEntity> _entitiesToAdd = new List<IGameEntity>();
        private readonly List<IGameEntity> _entitiesToRemove = new List<IGameEntity>();

        public IEnumerable<IGameEntity> Entities => new ReadOnlyCollection<IGameEntity>(_entities);

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                entity.Update(gameTime);
            }

            foreach (var entity in _entitiesToAdd)
            {
                _entities.Add(entity);
            }

            foreach (var entity in _entitiesToRemove)
            {
                _entities.Remove(entity);
            }
            
            _entitiesToAdd.Clear();
            _entitiesToRemove.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in _entities.OrderBy(e => e.DrawOrder))
            {
                entity.Draw(spriteBatch, gameTime);
            }
        }

        public void AddEntity(IGameEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Null cannot be added as an entity");
            }
            
            _entitiesToAdd.Add(entity);
        }

        public void RemoveEntity(IGameEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Null is not a valid entity");
            }
            
            _entitiesToRemove.Add(entity);
        }

        public void Clear()
        {
            _entitiesToRemove.AddRange(_entities);
        }
    }
}