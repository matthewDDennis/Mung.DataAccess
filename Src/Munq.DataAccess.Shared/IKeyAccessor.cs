using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Munq.DataAccess.Shared
{
    /// <summary>
    /// This class defined the interface for classes that provide access to the
    /// Key or Id property of an entity.
    /// </summary>
    /// <remarks>The Key property should be annotated with the <see cref="KeyAttribute"/>.</remarks>
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the Key.</typeparam>
    public interface IKeyAccessor<TEntity, TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets the value of the key property of the Entity.
        /// </summary>
        /// <param name="entity">The entity of interest.</param>
        /// <returns>The value of the Key property.</returns>
        TKey GetKey(TEntity entity);

        /// <summary>
        /// Sets the value of the key property of the Entity.
        /// </summary>
        /// <param name="entity">The entity of interest.</param>
        /// <param name="key">The value to set the key to.</param>
        /// <returns>The value of the Key property.</returns>
        TKey SetKey(TEntity entity, TKey key);

        /// <summary>
        /// Returns a new unique key value.
        /// </summary>
        /// <returns>The value of the new Key.</returns>
        TKey NextKey();
    }

    /// <summary>
    /// Implements to common functionality for KeyAccessors.
    /// </summary>
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TKey">The type of the Key.</typeparam>
    public abstract class KeyAccessorBase<TEntity, TKey> : IKeyAccessor<TEntity, TKey>
       where TEntity : class
       where TKey    : struct, IEquatable<TKey>
    {
        private static Func<TEntity, TKey>       _getKeyFunc = null;
        private static Func<TEntity, TKey, TKey> _setKeyFunc = null;



        static KeyAccessorBase()
        {
            PropertyInfo propertyInfo = GetKeyProperty();
            _getKeyFunc               = CreateGetFunc(propertyInfo);
            _setKeyFunc               = CreateSetFunc(propertyInfo);
        }

        /// <summary>
        /// Initializes an new instance of the <see cref="KeyAccessorBase{TEntity, TKey}"/> class.
        /// </summary>
        public KeyAccessorBase()
        {
        }

        /// <inheritdoc/>
        public TKey GetKey(TEntity entity)
        {
            if (_getKeyFunc is null)
                throw new InvalidOperationException(
                    "BuildAccessors hasn't been called to create the accessor functions.");

            return _getKeyFunc(entity);
        }

        /// <inheritdoc/>
        public TKey SetKey(TEntity entity, TKey key)
        {
            if (_setKeyFunc is null)
                throw new InvalidOperationException(
                    "BuildAccessors hasn't been called to create the accessor functions.");

            return _setKeyFunc(entity, key);
        }

        /// <inheritdoc/>
        public abstract TKey NextKey();

        private static PropertyInfo GetKeyProperty()
        {
            var props = typeof(TEntity).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));

            if (!props.Any())
                throw new MissingMemberException(
                    $"The Type {typeof(TEntity).Name} does not have a Property with the Key Attribute");

            if (props.Count() > 1)
                throw new InvalidOperationException($"Multiple Key Properties are not supported.");

            return props.Single();
        }

        private static Func<TEntity, TKey> CreateGetFunc(PropertyInfo propertyInfo)
        {
            return (TEntity entity) => (TKey)propertyInfo.GetValue(entity);
        }

        private static Func<TEntity, TKey, TKey> CreateSetFunc(PropertyInfo propertyInfo)
        {
            return (TEntity entity, TKey value) =>
                {
                    propertyInfo.SetValue(entity, value);
                    return (TKey) value;
                };
        }
    }

    /// <summary>
    /// Defines the implementation for an Int Key Accessor
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// </summary>
    public class IntKeyAccessor<TEntity> : KeyAccessorBase<TEntity, int>
        where TEntity : class
    {
        static int _nextKey = 0;

        /// <inheritdoc/>
        public override int NextKey()
        {
            if (_nextKey == int.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Unsigned Int Key Accessor
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// </summary>
    public class UIntKeyAccessor<TEntity> : KeyAccessorBase<TEntity, uint>
        where TEntity : class
    {
        static uint _nextKey = 0;

        /// <inheritdoc/>
        public override uint NextKey()
        {
            if (_nextKey == uint.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Long Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class LongKeyAccessor<TEntity> : KeyAccessorBase<TEntity, long>
        where TEntity : class
    {
        static long _nextKey = 0;

        /// <inheritdoc/>
        public override long NextKey()
        {
            if (_nextKey == long.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Unsigned Long Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class ULongKeyAccessor<TEntity> : KeyAccessorBase<TEntity, ulong>
        where TEntity : class
    {
        static ulong _nextKey = 0;

        /// <inheritdoc/>
        public override ulong NextKey()
        {
            if (_nextKey == ulong.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Short Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class ShortKeyAccessor<TEntity> : KeyAccessorBase<TEntity, short>
        where TEntity : class
    {
        static int _nextKey = 0;

        /// <inheritdoc/>
        public override short NextKey()
        {
            if (_nextKey == short.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return (short)Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Unsigned Short Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class UShortKeyAccessor<TEntity> : KeyAccessorBase<TEntity, ushort>
        where TEntity : class
    {
        static uint _nextKey = 0;

        /// <inheritdoc/>
        public override ushort NextKey()
        {
            if (_nextKey == ushort.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return (ushort)Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Byte Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class ByteKeyAccessor<TEntity> : KeyAccessorBase<TEntity, byte>
        where TEntity : class
    {
        static int _nextKey = 0;

        /// <inheritdoc/>
        public override byte NextKey()
        {
            if (_nextKey == byte.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return (byte)Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Signed Byte Key Accessor
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class SByteKeyAccessor<TEntity> : KeyAccessorBase<TEntity, sbyte>
        where TEntity : class
    {
        static int _nextKey = 0;

        /// <inheritdoc/>
        public override sbyte NextKey()
        {
            if (_nextKey == ushort.MaxValue)
                throw new IndexOutOfRangeException("All the possible key values have been used.");

            return (sbyte)Interlocked.Increment(ref _nextKey);
        }
    }

    /// <summary>
    /// Defines the implementation for an Guid Key Accessor
    /// </summary>
	/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public class GuidKeyAccessor<TEntity> : KeyAccessorBase<TEntity,Guid>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of thee <see cref="GuidKeyAccessor{TEntity}"/> class.
        /// </summary>
        public GuidKeyAccessor()
        {
        }

        /// <inheritdoc/>
        public override Guid NextKey() => Guid.NewGuid();
    }

    /// <summary>
    /// A Factory for creating the KeyAccessors.
    /// </summary>
    public static class KeyAccessorFactory
    {
        /// <summary>
        /// Creates the implementation for <see cref="IKeyAccessor{TEntity, TKey}"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IKeyAccessor<TEntity, TKey> Create<TEntity, TKey>()
            where TEntity : class
            where TKey    : struct, IEquatable<TKey>
        {
            TKey key = default;
            object accessor = key switch
            {
                int    => new IntKeyAccessor<TEntity>(),
                uint   => new UIntKeyAccessor<TEntity>(),
                long   => new LongKeyAccessor<TEntity>(),
                ulong  => new ULongKeyAccessor<TEntity>(),
                short  => new ShortKeyAccessor<TEntity>(),
                ushort => new UShortKeyAccessor<TEntity>(),
                byte   => new ByteKeyAccessor<TEntity>(),
                sbyte  => new SByteKeyAccessor<TEntity>(),
                Guid   => new GuidKeyAccessor<TEntity>(),
                _      => throw new Exception($"{typeof(TKey).Name} is not supported.")
            };

            if (accessor is KeyAccessorBase<TEntity, TKey> typedAccessor)
            {
                return typedAccessor;
            }
            else
                throw new Exception($"Unable to create a KeyAccessor for type {typeof(TKey).Name}.");
        }

    }
}
