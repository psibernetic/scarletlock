﻿using System;

namespace ScarletLock
{
    public class DistributedLockFactory<TIdentity> 
        : IDistributedLockFactory<TIdentity>
    {
        protected Func<TIdentity> IdentityGenerator { get; }

        protected DistributedLockFactory(Func<TIdentity> identityGenerator)
        {
            IdentityGenerator = identityGenerator;
        }

        public virtual IDistributedLock<TIdentity> EstablishLock(IDistributedLockManager<TIdentity> dlm, PreliminaryLock<TIdentity> preliminaryLock, DateTime expiration)
        {
            return DistributedLock<TIdentity>.EstablishLock(dlm, preliminaryLock.Resource, preliminaryLock.Identity, expiration);
        }

        public virtual PreliminaryLock<TIdentity> GetPreliminaryLock(string resoure)
        {
            return new PreliminaryLock<TIdentity>(IdentityGenerator(), resoure);
        }

        protected virtual TIdentity GenerateIdentity()
        {
            try
            {
                var identity = IdentityGenerator();

                if (identity.Equals(default(TIdentity)))
                    throw new IdentityGenerationException("Identity generatator provided default value.");

                return identity;
            }
            catch (Exception ex) when (!(ex is IdentityGenerationException))
            {
                throw new IdentityGenerationException("An exception occured in identity generator", ex);
            }
        }

        public static IDistributedLockFactory<TIdentity> Create(Func<TIdentity> identityGenerator)
        {
            return new DistributedLockFactory<TIdentity>(identityGenerator);
        }
    }
}
