import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import Login from './Login';
import Home from './Home';

import logo from './logo.svg';
import './App.css';
class App extends Component {
  render() {
    return (
      <Router>
        <div>
          <Route exact path="/" component={Home} />
          <Route path="/login" component={Login} />
        </div>
      </Router>
    );
  }
}

export default App;
