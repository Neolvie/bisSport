using System;
using System.Collections.Generic;
using System.Linq;
using bisSport.Domain.Core;
using bisSport.Domain.Core.Base;
using bisSport.Domain.Core.Interfaces;

namespace bisSport.Domain.Helpers
{
  public static class CoreTypes
  {
    public static readonly Dictionary<Type, string> CoreTypeGuids = new Dictionary<Type, string>()
    {
      { typeof(Entity), "73AD9DE5-7B26-4DD6-80B3-1C70864C95C3" },
      { typeof(EventBase), "3FFDE302-FC15-44BF-BAA2-9308D82588D9" },
      { typeof(OrganizerBase), "C89CE02C-D522-44BE-9BA4-C3E94FC5A99A" },
      { typeof(ParticipantBase), "774B9919-1C11-4103-B94F-8920296E923E" },
      { typeof(Address), "ED63B7AF-DE05-495A-BA5D-ED11B35516BF" },
      { typeof(Arbiter), "B8F7742B-A00B-435C-A536-CFAB2D71ACCC" },
      { typeof(Area), "8314C9B3-08D0-4435-93F5-CDC5BB425D29" },
      { typeof(Coach), "EF71B28C-9486-4EBC-ABF3-C5741BEE2ABF" },
      { typeof(Match), "D86098DB-0C07-46D9-954D-AF925210D43F" },
      { typeof(MultiEvent), "2829A68D-7D4A-458F-873A-09BDAB566434" },
      { typeof(Player), "EC96138A-1591-4207-86B7-8335B20019DC" },
      { typeof(Point), "DB25230F-3420-4FAB-B052-8CD38B8F9D3F" },
      { typeof(Result), "2E969BB3-162C-47C9-A211-E3BAE0958C9F" },
      { typeof(Round), "7BA9EB58-108D-4556-AC24-EA869D640387" },
      { typeof(Score), "A363AC8D-F932-4B6E-9A7F-E3A375906DB4" },
      { typeof(SingleEvent), "3288FC3E-0490-4DF9-83B6-0E5F8573A93A" },
      { typeof(SportClub), "FD594871-02D9-4027-8701-3BEFC8E0AA06" },
      { typeof(SportType), "39C531A6-8A41-45DB-90AB-86DFA26A40E4" },
      { typeof(Structure), "92BB49AB-055C-43E3-8D0A-F27286F45F1A" },
      { typeof(Team), "72396685-C65C-48ED-81B0-1B87DFF6577B" },
      { typeof(Group), "017440E7-3260-4AE1-92D2-3746622010F4" },
      { typeof(Scorer), "A2C7F9BA-3B57-40B0-BE3D-F8CB72C3E07C" }
    };

    public static string GetEntityGuid(Type type)
    {
      string guid;

      if (CoreTypeGuids.TryGetValue(type, out guid))
        return guid;

      if (type.BaseType != null && CoreTypeGuids.TryGetValue(type.BaseType, out guid))
        return guid;

      throw new Exception("No entity GUID found");
    }

    public static string GetEntityGuid(this IEntity entity)
    {
      return GetEntityGuid(entity.GetType());
    }

    public static Type GetEntityType(string guid)
    {
      var type = CoreTypeGuids.SingleOrDefault(x => x.Value == guid);

      if (Equals(type, null))
        throw new Exception("No entity GUID found");

      return type.Key;
    }

    public static IEntity CreateEntityInstance(string guid)
    {
      var type = CoreTypeGuids.SingleOrDefault(x => x.Value.Equals(guid, StringComparison.InvariantCultureIgnoreCase)).Key;
      if (type == null)
        throw new Exception("Error creating instance. No entity type found.");

      return (IEntity)Activator.CreateInstance(type);
    }

    public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
      while (toCheck != null && toCheck != typeof(object))
      {
        var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        if (generic == cur)
        {
          return true;
        }
        toCheck = toCheck.BaseType;
      }
      return false;
    }
  }
}