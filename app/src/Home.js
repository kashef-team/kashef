import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import TopAppBar from './TopAppBar';

const styles = {

};

class Home extends Component {
    render() {
      return (
        <div>
            Home
        </div>
      );
    }
  }
  
  export default withStyles(styles)(Home);