using Box2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box2D.Common
{
    public class QueryCallback : b2QueryCallback
    {
        public QueryCallback(b2Vec2 point)
        {
            m_point = point;
            m_fixture = null;
        }

        public override bool ReportFixture(b2Fixture fixture)
        {
            b2Body body = fixture.Body;
            if (body.BodyType == b2BodyType.b2_dynamicBody)
            {
                bool inside = fixture.TestPoint(m_point);
                if (inside)
                {
                    m_fixture = fixture;

                    // We are done, terminate the query.
                    return false;
                }
            }

            // Continue the query.
            return true;
        }

        public b2Vec2 m_point;
        public b2Fixture m_fixture;
    }

}
