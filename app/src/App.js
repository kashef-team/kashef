import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import TopAppBar from './TopAppBar';

import logo from './logo.svg';
import './App.css';
class App extends Component {
  render() {
    return (
      <TopAppBar />
    );
  }
}

export default App;
