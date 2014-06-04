﻿using pacmanduelbot.models;
using pacmanduelbot.shared;
using System.Collections.Generic;
using System.Drawing;

namespace pacmanduelbot.helpers
{
    class Moves
    {
        public static List<Point> NextPossiblePositions(char[][] maze, Point currentPoint)
        {
            var moveList = new List<Point>();
            if (currentPoint.Y + 1 < Guide._WIDTH
                && !maze[currentPoint.X][currentPoint.Y + 1].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._FORBIDDEN_R_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_R_Y - 1)))
                moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });

            if (currentPoint.Y - 1 >= 0
                && !maze[currentPoint.X][currentPoint.Y - 1].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._FORBIDDEN_L_X) && currentPoint.Y.Equals(Guide._FORBIDDEN_L_Y + 1)))
                moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });

            if (currentPoint.X + 1 < Guide._HEIGHT
                && !maze[currentPoint.X + 1][currentPoint.Y].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._EXIT_UP_X - 1) && currentPoint.Y.Equals(Guide._EXIT_UP_Y))
                && !(currentPoint.X.Equals(Guide._RESPAWN_X - 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
            {
                if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                    && maze[currentPoint.X + 1][currentPoint.Y].Equals(Guide._OPPONENT_SYMBOL))
                {
                    //do nothing          
                }
                else
                {
                    moveList.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });
                }
            }

            if (currentPoint.X - 1 >= 0
                && !maze[currentPoint.X - 1][currentPoint.Y].Equals(Guide._WALL)
                && !(currentPoint.X.Equals(Guide._EXIT_DOWN_X + 1) && currentPoint.Y.Equals(Guide._EXIT_DOWN_Y))
                && !(currentPoint.X.Equals(Guide._RESPAWN_X + 1) && currentPoint.Y.Equals(Guide._RESPAWN_Y)))
            {
                if ((currentPoint.X.Equals(Guide._RESPAWN_X) && currentPoint.Y.Equals(Guide._RESPAWN_Y))
                    && maze[currentPoint.X - 1][currentPoint.Y].Equals(Guide._OPPONENT_SYMBOL))
                {
                    //do nothing
                }
                else
                {
                    moveList.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });
                }
            }

            if (currentPoint.X.Equals(Guide._PORTAL1_X) && currentPoint.Y.Equals(Guide._PORTAL1_Y))
                moveList.Add(new Point { X = Guide._PORTAL2_X, Y = Guide._PORTAL2_Y });

            if (currentPoint.X.Equals(Guide._PORTAL2_X) && currentPoint.Y.Equals(Guide._PORTAL2_Y))
                moveList.Add(new Point { X = Guide._PORTAL1_X, Y = Guide._PORTAL1_Y });

            return moveList;
        }

        public static Point ChoosePath(char[][] _maze, Point _current_position, int _depth)
        {
            var _next = new Point();
            var _open = new List<Node>();
            var _closed = new List<Node>();

            var _node = new Node { _position = _current_position };
            
            _open.Add(_node);

            var _count = 0;

            while (_open.Count != 0 && _count < _depth)
            {
                var _open_root = _open[0];
                _closed.Add(_open_root);

                var _tempI = NextPossiblePositions(_maze, _open_root._position);
                
                foreach (var _point in  _tempI)
                {
                    var _case = _maze[_point.X][_point.Y];
                    if (_open_root._parent != null)
                    {
                        if (!(_point.X == _open_root._parent._position.X && _point.Y == _open_root._parent._position.Y))
                        {
                            switch(_case)
                            {
                                case Guide._PILL:
                                    var _path_node = new Node
                                    { 
                                        _position = _point,
                                        _score=_open_root._score + 1,
                                        _isLeaf=isLeaf(_maze,_point,_open_root._position),
                                        _parent = _open_root
                                    };
                                    _open.Add(_path_node);
                                    break;
                                case Guide._BONUS_PILL:
                                    _path_node = new Node
                                    {
                                        _position = _point,
                                        _score=_open_root._score + 10,
                                        _isLeaf = isLeaf(_maze, _point, _open_root._position),
                                        _parent = _open_root
                                    };
                                    _open.Add(_path_node);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (_case)
                        {
                            case Guide._PILL:
                                var _path_node = new Node
                                {
                                    _position = _point,
                                    _score = _open_root._score + 1,
                                    _isLeaf = isLeaf(_maze,_point,_open_root._position),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            case Guide._BONUS_PILL:
                                _path_node = new Node
                                {
                                    _position = _point,
                                    _score = _open_root._score + 10,
                                    _isLeaf = isLeaf(_maze,_point,_open_root._position),
                                    _parent = _open_root
                                };
                                _open.Add(_path_node);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _open.Remove(_open_root);
                _count++;
            }

            var curr = new Node();
            var _closed_root = _closed[0];

            if (!(_open.Count == 0))
            {
                foreach (var _item in _closed)
                {
                    if (_item._score > curr._score)
                    {
                        curr = _item;
                    }
                }
                while (!(curr._parent._position.X == _closed_root._position.X && curr._parent._position.Y == _closed_root._position.Y))
                    curr = curr._parent;
                _next = curr._position;
                return _next;
            }

            foreach (var _item in _closed)
            {
                if (_item._isLeaf)
                {
                    if (_item._score == 1)
                    {
                        _next = _item._position;
                        return _next;
                    }
                    else
                    {
                        if (_item._score > curr._score)
                        {
                            curr = _item;
                        }
                    }
                }
            }

            while (!(curr._parent._position.X == _closed_root._position.X && curr._parent._position.Y == _closed_root._position.Y))
                curr = curr._parent;
            _next = curr._position;
            return _next;
        }

        private static bool isLeaf(char[][] _maze, Point _point, Point _parent)
        {
            var _isLeaf = true;
            var _list = NextPossiblePositions(_maze, _point);

            foreach (var _item in  _list)
            {
                if (!(_item.X == _parent.X && _item.Y == _parent.Y)
                    && (_maze[_item.X][_item.Y].Equals(Guide._BONUS_PILL)
                    || _maze[_item.X][_item.Y].Equals(Guide._PILL)))
                {
                    _isLeaf = false;
                    break;
                }
            }
            return _isLeaf;
        }
        
        public static List<Point> BuildPath(char[][] _maze, Point _start, Point _goal)
        {
            var _list = new List<Point>();
            var _open = new List<Node>();
            var _closed = new List<Node>();

            var _gG = 0;
            var _hH = Mappings.ManhattanDistance(_start, _goal);
            var _fF = _gG + _hH;
            var _node = new Node { _position = _start, _g = _gG, _h = _hH, _f = _fF };

            _open.Add(_node);
                        
            while (_open.Count != 0)
            {
                var _current = LowestRank(_open);                
                if ((_current._position.X == _goal.X)
                    && (_current._position.Y == _goal.Y))
                {
                    _list.Add(new Point { X = _current._g });
                    //traverse back
                    while (!(_current._parent._position.X==_start.X
                           &&_current._parent._position.Y==_start.Y))
                    {
                        _current = _current._parent;
                    }
                    _list.Add(_current._position);
                    //_list = _current._position;
                    break;
                }

                _closed.Add(_current);
                _open.Remove(_current);//remove it from open list

                var _neighbors = NextPossiblePositions(_maze, _current._position);
                
                foreach(var _neighbor in _neighbors)
                {
                    _gG = _current._g + 1;
                    _hH = Mappings.ManhattanDistance(_neighbor, _goal);
                    _fF = _gG + _hH;
                    var _curr = new Node { _position = _neighbor, _g = _gG, _h = _hH, _f = _fF, _parent = _current };
                    if (!_closed.Contains(_curr))
                    {
                        //add it to open list
                        _open.Add(_curr);
                    }
                }
            }
            return _list;
        }

        /*
        private static bool Contains(List<PathNode> _nodes, PathNode _node)
        {
            bool _found = false;
            for (var i = 0; i < _nodes.Count;i++)
            {
                if (_nodes[i]._position.X == _node._position.X && _nodes[i]._position.Y == _node._position.Y)
                {
                    _found = true;
                    break;
                }
            }
                return _found;
        }*/

        private static Node LowestRank(List<Node> _nodes)
        {
            var _result = new Node();
            if (_nodes.Count == 0)
            {
                _result = null;
            }
            else
            {
                foreach(var _node in _nodes)
                {
                    if (_result._position.IsEmpty)
                        _result = _node;
                    else
                    {
                        if (_node._f < _result._f)
                            _result = _node;
                    }
                }
            }
            return _result;
        }
    }
}