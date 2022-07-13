using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace GeneratorBase.MVC.Models
{
/// <summary>A database set helper.</summary>
public static class DbSetHelper
{
    /// <summary>An IDbSet&lt;TEntity&gt; extension method that unfiltered the given set.</summary>
    ///
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <param name="set">The set to act on.</param>
    ///
    /// <returns>An IQueryable&lt;TEntity&gt;</returns>
    
    public static IQueryable<TEntity> Unfiltered<TEntity>(this IDbSet<TEntity> set) where TEntity : class
    {
        var filteredDbSet = set as FilteredDbSet<TEntity>;
        return filteredDbSet != null ? filteredDbSet.UnfilteredData() : set;
    }
}

/// <summary>A filtered database set.</summary>
///
/// <typeparam name="TEntity">Type of the entity.</typeparam>

public class FilteredDbSet<TEntity> : IDbSet<TEntity>, IOrderedQueryable<TEntity>, IOrderedQueryable, IQueryable<TEntity>, IQueryable, IEnumerable<TEntity>, IEnumerable, IListSource
    where TEntity : class
{
    /// <summary>The set.</summary>
    private readonly DbSet<TEntity> _set;
    /// <summary>The initialize entity.</summary>
    private readonly Action<TEntity> _initializeEntity;
    /// <summary>Specifies the filter.</summary>
    private readonly Expression<Func<TEntity, bool>> _filter;
    //---------------------------------------------------------------------------
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="context">The context.</param>
    
    public FilteredDbSet(DbContext context)
        : this(context.Set<TEntity>(), i => true, null)
    {
    }
    
    /// <summary>Unfiltered data.</summary>
    ///
    /// <returns>An IQueryable&lt;TEntity&gt;</returns>
    
    public IQueryable<TEntity> UnfilteredData()
    {
        return _set;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="context">The context.</param>
    /// <param name="filter"> Specifies the filter.</param>
    
    public FilteredDbSet(DbContext context, Expression<Func<TEntity, bool>> filter)
        : this(context.Set<TEntity>(), filter, null)
    {
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="context">         The context.</param>
    /// <param name="filter">          Specifies the filter.</param>
    /// <param name="initializeEntity">The initialize entity.</param>
    
    public FilteredDbSet(DbContext context, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity)
        : this(context.Set<TEntity>(), filter, initializeEntity)
    {
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets the filter.</summary>
    ///
    /// <value>The filter.</value>
    
    public Expression<Func<TEntity, bool>> Filter
    {
        get
        {
            return _filter;
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Includes the given file.</summary>
    ///
    /// <param name="path">Full pathname of the file.</param>
    ///
    /// <returns>An IQueryable&lt;TEntity&gt;</returns>
    
    public IQueryable<TEntity> Include(string path)
    {
        return _set.Include(path).Where(_filter).AsQueryable();
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="set">             The set.</param>
    /// <param name="filter">          Specifies the filter.</param>
    /// <param name="initializeEntity">The initialize entity.</param>
    
    private FilteredDbSet(DbSet<TEntity> set, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity)
    {
        _set = set;
        _filter = filter;
        MatchesFilter = filter.Compile();
        _initializeEntity = initializeEntity;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets or sets the matches filter.</summary>
    ///
    /// <value>A function delegate that yields a bool.</value>
    
    public Func<TEntity, bool> MatchesFilter
    {
        get;
        private set;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets the unfiltered.</summary>
    ///
    /// <returns>An IQueryable&lt;TEntity&gt;</returns>
    
    public IQueryable<TEntity> Unfiltered()
    {
        return _set;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Throw if entity does not match filter.</summary>
    ///
    /// <param name="entity">The entity to add.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    
    public bool ThrowIfEntityDoesNotMatchFilter(TEntity entity)
    {
        try
        {
            if(!MatchesFilter(entity))
            {
                var context = new System.Web.Routing.RequestContext(
                    new HttpContextWrapper(System.Web.HttpContext.Current),
                    new RouteData());
                var urlHelper = new UrlHelper(context);
                var url = urlHelper.Action("Index", "Error");
                System.Web.HttpContext.Current.Response.Redirect(url);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return false;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Adds the given entity to the context underlying the set in the Added state such that
    /// it will be inserted into the database when SaveChanges is called.</summary>
    ///
    /// <remarks>Note that entities that are already in the context in some other state will have
    /// their state set to Added.  Add is a no-op if the entity is already in the context in the
    /// Added state.</remarks>
    ///
    /// <param name="entity">The entity to add.</param>
    ///
    /// <returns>The entity.</returns>
    
    public TEntity Add(TEntity entity)
    {
        DoInitializeEntity(entity);
        // if (ThrowIfEntityDoesNotMatchFilter(entity)) return null;
        return _set.Add(entity);
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Attaches the given entity to the context underlying the set.  That is, the entity is
    /// placed into the context in the Unchanged state, just as if it had been read from the database.</summary>
    ///
    /// <remarks>Attach is used to repopulate a context with an entity that is known to already exist
    /// in the database. SaveChanges will therefore not attempt to insert an attached entity into the
    /// database because it is assumed to already be there. Note that entities that are already in
    /// the context in some other state will have their state set to Unchanged.  Attach is a no-op if
    /// the entity is already in the context in the Unchanged state.</remarks>
    ///
    /// <param name="entity">The entity to attach.</param>
    ///
    /// <returns>The entity.</returns>
    
    public TEntity Attach(TEntity entity)
    {
        ThrowIfEntityDoesNotMatchFilter(entity);
        return _set.Attach(entity);
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Creates a new instance of an entity for the type of this set or for a type derived
    /// from the type of this set. Note that this instance is NOT added or attached to the set. The
    /// instance returned will be a proxy if the underlying context is configured to create proxies
    /// and the entity type meets the requirements for creating a proxy.</summary>
    ///
    /// <typeparam name="TDerivedEntity">The type of entity to create.</typeparam>
    ///
    /// <returns>The entity instance, which may be a proxy.</returns>
    
    public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
    {
        var entity = _set.Create<TDerivedEntity>();
        DoInitializeEntity(entity);
        return (TDerivedEntity)entity;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Creates a new instance of an entity for the type of this set. Note that this instance
    /// is NOT added or attached to the set. The instance returned will be a proxy if the underlying
    /// context is configured to create proxies and the entity type meets the requirements for
    /// creating a proxy.</summary>
    ///
    /// <returns>The entity instance, which may be a proxy.</returns>
    
    public TEntity Create()
    {
        var entity = _set.Create();
        DoInitializeEntity(entity);
        return entity;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Finds an entity with the given primary key values. If an entity with the given
    /// primary key values exists in the context, then it is returned immediately without making a
    /// request to the store.  Otherwise, a request is made to the store for an entity with the given
    /// primary key values and this entity, if found, is attached to the context and returned.  If no
    /// entity is found in the context or the store, then null is returned.</summary>
    ///
    /// <remarks>The ordering of composite key values is as defined in the EDM, which is in turn as
    /// defined in the designer, by the Code First fluent API, or by the DataMember attribute.</remarks>
    ///
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    ///
    /// <returns>The entity found, or null.</returns>
    
    public TEntity Find(params object[] keyValues)
    {
        var entity = _set.Find(keyValues);//.Where(this.Filter)
        if(entity == null)
            return null;
        // return this._set.Find(keyValues);
        // If the user queried an item outside the filter, then we throw an error.
        // If IDbSet had a Detach method we would use it...sadly, we have to be ok with the item being in the Set.
        ThrowIfEntityDoesNotMatchFilter(entity);
        return entity;
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Marks the given entity as Deleted such that it will be deleted from the database when
    /// SaveChanges is called.  Note that the entity must exist in the context in some other state
    /// before this method is called.</summary>
    ///
    /// <remarks>Note that if the entity exists in the context in the Added state, then this method
    /// will cause it to be detached from the context.  This is because an Added entity is assumed
    /// not to exist in the database such that trying to delete it does not make sense.</remarks>
    ///
    /// <param name="entity">The entity to remove.</param>
    ///
    /// <returns>The entity.</returns>
    
    public TEntity Remove(TEntity entity)
    {
        ThrowIfEntityDoesNotMatchFilter(entity);
        return _set.Remove(entity);
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Returns the items in the local cache.</summary>
    ///
    /// <remarks>It is possible to add/remove entities via this property that do NOT match the filter.
    /// Use the <see cref="ThrowIfEntityDoesNotMatchFilter"/> method before adding/removing an item
    /// from this collection.</remarks>
    ///
    /// <value>The local.</value>
    
    public ObservableCollection<TEntity> Local
    {
        get
        {
            return _set.Local;
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    ///
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    ///
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to
    /// iterate through the collection.</returns>
    
    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
    {
        return _set.Where(_filter).GetEnumerator();
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    ///
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
    /// through the collection.</returns>
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _set.Where(_filter).GetEnumerator();
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets the type of the element(s) that are returned when the expression tree associated
    /// with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.</summary>
    ///
    /// <value>A <see cref="T:System.Type" /> that represents the type of the element(s) that are
    /// returned when the expression tree associated with this object is executed.</value>
    
    Type IQueryable.ElementType
    {
        get
        {
            return typeof(TEntity);
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets the expression tree that is associated with the instance of
    /// <see cref="T:System.Linq.IQueryable" />.</summary>
    ///
    /// <value>The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this
    /// instance of <see cref="T:System.Linq.IQueryable" />.</value>
    
    Expression IQueryable.Expression
    {
        get
        {
            return _set.Where(_filter).Expression;
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets the query provider that is associated with this data source.</summary>
    ///
    /// <value>The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data
    /// source.</value>
    
    IQueryProvider IQueryable.Provider
    {
        get
        {
            return _set.AsQueryable().Provider;
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Gets a value indicating whether the collection is a collection of
    /// <see cref="T:System.Collections.IList" /> objects.</summary>
    ///
    /// <value>true if the collection is a collection of <see cref="T:System.Collections.IList" />
    /// objects; otherwise, false.</value>
    
    bool IListSource.ContainsListCollection
    {
        get
        {
            return false;
        }
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Returns an <see cref="T:System.Collections.IList" /> that can be bound to a data
    /// source from an object that does not implement an <see cref="T:System.Collections.IList" />
    /// itself.</summary>
    ///
    /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
    ///                                                 invalid.</exception>
    ///
    /// <returns>An <see cref="T:System.Collections.IList" /> that can be bound to a data source from
    /// the object.</returns>
    
    IList IListSource.GetList()
    {
        throw new InvalidOperationException();
    }
    //---------------------------------------------------------------------------
    
    /// <summary>Executes the initialize entity operation.</summary>
    ///
    /// <param name="entity">The entity.</param>
    
    void DoInitializeEntity(TEntity entity)
    {
        if(_initializeEntity != null)
            _initializeEntity(entity);
    }
    //---------------------------------------------------------------------------
    
    /// <summary>SQL query.</summary>
    ///
    /// <param name="sql">       The SQL.</param>
    /// <param name="parameters">A variable-length parameters list containing parameters.</param>
    ///
    /// <returns>A DbSqlQuery&lt;TEntity&gt;</returns>
    
    public DbSqlQuery<TEntity> SqlQuery(string sql, params object[] parameters)
    {
        return _set.SqlQuery(sql, parameters);
    }
}
}